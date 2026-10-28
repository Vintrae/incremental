using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Incremental.Game.Scripts.Upgrades;

namespace Incremental.Game.Scripts.Services;

public sealed class UpgradeService
{
    private readonly Dictionary<string, UpgradeEntry> _attributes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, UpgradeDefinition> _definitions = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> _levels = new(StringComparer.Ordinal);

    public event Action UpgradeChanged;

    public UpgradeService()
    {
        foreach (UpgradeDefinition definition in UpgradeIds.All)
        {
            if (!_definitions.TryAdd(definition.Id, definition))
                throw new InvalidOperationException($"The upgrade ID '{definition.Id}' is already defined.");

            _levels.Add(definition.Id, 0);
        }
    }

    public void Register(object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        Type targetType = target.GetType();
        foreach (FieldInfo field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            var attribute = field.GetCustomAttribute<UpgradableAttribute>();
            if (attribute != null)
                AddAttribute(attribute, target, field);
        }

        foreach (PropertyInfo property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || !property.CanWrite)
                continue;

            var attribute = property.GetCustomAttribute<UpgradableAttribute>();
            if (attribute != null)
                AddAttribute(attribute, target, property);
        }
    }

    public bool TryGet(string upgradeId, out UpgradeInfo info)
    {
        if (_definitions.TryGetValue(upgradeId, out UpgradeDefinition definition) &&
            _attributes.TryGetValue(definition.AttributeId, out UpgradeEntry entry))
        {
            info = CreateInfo(definition, _levels[upgradeId], entry);
            return true;
        }

        info = null;
        return false;
    }

    public IReadOnlyList<UpgradeInfo> GetAll()
    {
        var upgrades = new List<UpgradeInfo>(_definitions.Count);
        foreach (UpgradeDefinition definition in UpgradeIds.All)
        {
            if (!TryGet(definition.Id, out UpgradeInfo info))
                throw new InvalidOperationException(
                    $"Upgrade '{definition.Id}' references an attribute that has not been registered.");

            upgrades.Add(info);
        }

        return upgrades;
    }

    public bool TryPurchase(string upgradeId, IScoreWallet scoreWallet)
    {
        ArgumentNullException.ThrowIfNull(scoreWallet);

        if (!_definitions.TryGetValue(upgradeId, out UpgradeDefinition definition) ||
            !_attributes.TryGetValue(definition.AttributeId, out UpgradeEntry entry) ||
            _levels[upgradeId] >= definition.MaxLevel)
            return false;

        float current = Convert.ToSingle(GetValue(entry), CultureInfo.InvariantCulture);
        float next = MathF.Min(current + definition.IncreaseAmount, entry.Attribute.MaxValue);
        if (next <= current)
            return false;

        object convertedValue = Convert.ChangeType(next, GetMemberType(entry.Member), CultureInfo.InvariantCulture);
        if (Convert.ToSingle(convertedValue, CultureInfo.InvariantCulture) <= current)
            return false;

        double cost = definition.GetCostForLevel(_levels[upgradeId]);
        if (!scoreWallet.TrySpendScore(cost))
            return false;

        SetValue(entry, convertedValue);
        _levels[upgradeId]++;
        UpgradeChanged?.Invoke();
        return true;
    }

    private void AddAttribute(UpgradableAttribute attribute, object target, MemberInfo member)
    {
        Type memberType = member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => throw new ArgumentException("Only fields and properties can be upgradable.", nameof(member))
        };
        if (!IsSupportedNumericType(memberType))
            throw new ArgumentException($"Upgradable member '{member.Name}' must have a numeric type.", nameof(member));

        if (!_attributes.TryAdd(attribute.AttributeId, new UpgradeEntry(target, member, attribute)))
            throw new InvalidOperationException($"The attribute ID '{attribute.AttributeId}' is already registered.");
    }

    private static UpgradeInfo CreateInfo(UpgradeDefinition definition, int level, UpgradeEntry entry)
    {
        return new UpgradeInfo(
            definition.Id,
            definition.DisplayName,
            definition.AttributeId,
            Convert.ToSingle(GetValue(entry), CultureInfo.InvariantCulture),
            entry.Attribute.MaxValue,
            definition.IncreaseAmount,
            level,
            definition.MaxLevel,
            level < definition.MaxLevel ? definition.GetCostForLevel(level) : null);
    }

    private static object GetValue(UpgradeEntry entry) => entry.Member switch
    {
        FieldInfo field => field.GetValue(entry.Target),
        PropertyInfo property => property.GetValue(entry.Target),
        _ => null
    };

    private static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => throw new ArgumentException("Only fields and properties can be upgradable.", nameof(member))
        };
    }

    private static void SetValue(UpgradeEntry entry, object value)
    {
        switch (entry.Member)
        {
            case FieldInfo field:
                field.SetValue(entry.Target, value);
                break;
            case PropertyInfo property:
                property.SetValue(entry.Target, value);
                break;
        }
    }

    private static bool IsSupportedNumericType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }

    private sealed record UpgradeEntry(object Target, MemberInfo Member, UpgradableAttribute Attribute);
}

public sealed record UpgradeInfo(
    string UpgradeId,
    string DisplayName,
    string AttributeId,
    float CurrentValue,
    float MaxValue,
    float IncreaseAmount,
    int Level,
    int MaxLevel,
    double? NextCost);
