using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Incremental.Game.Scripts.Upgrades;

public sealed class UpgradeManager
{
    private readonly Dictionary<string, UpgradeEntry> _upgrades = new(StringComparer.Ordinal);

    public void Register(object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        Type targetType = target.GetType();
        foreach (FieldInfo field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            var attribute = field.GetCustomAttribute<UpgradableAttribute>();
            if (attribute != null)
                AddEntry(attribute, target, field);
        }

        foreach (PropertyInfo property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || !property.CanWrite)
                continue;

            var attribute = property.GetCustomAttribute<UpgradableAttribute>();
            if (attribute != null)
                AddEntry(attribute, target, property);
        }
    }

    public bool TryGet(string upgradeId, out UpgradeInfo info)
    {
        if (_upgrades.TryGetValue(upgradeId, out UpgradeEntry entry))
        {
            info = CreateInfo(entry);
            return true;
        }

        info = null;
        return false;
    }

    public bool TryUpgrade(string upgradeId, float amount)
    {
        if (amount <= 0f || !_upgrades.TryGetValue(upgradeId, out UpgradeEntry entry))
            return false;

        object currentValue = GetValue(entry);
        if (!TryConvertToFloat(currentValue, out float current))
            return false;

        float next = MathF.Min(current + amount, entry.Attribute.MaxValue);
        if (next <= current)
            return false;

        SetValue(entry, next);
        return true;
    }

    private void AddEntry(UpgradableAttribute attribute, object target, MemberInfo member)
    {
        if (string.IsNullOrWhiteSpace(attribute.UpgradeId))
            throw new ArgumentException("An upgrade ID cannot be empty.", nameof(attribute));

        if (!_upgrades.TryAdd(attribute.UpgradeId, new UpgradeEntry(target, member, attribute)))
            throw new InvalidOperationException($"The upgrade ID '{attribute.UpgradeId}' is already registered.");
    }

    private static UpgradeInfo CreateInfo(UpgradeEntry entry)
    {
        object value = GetValue(entry);
        float currentValue = TryConvertToFloat(value, out float converted) ? converted : 0f;
        return new UpgradeInfo(
            entry.Attribute.UpgradeId,
            currentValue,
            entry.Attribute.MaxValue,
            entry.Attribute.Cost);
    }

    private static object GetValue(UpgradeEntry entry) => entry.Member switch
    {
        FieldInfo field => field.GetValue(entry.Target),
        PropertyInfo property => property.GetValue(entry.Target),
        _ => null
    };

    private static void SetValue(UpgradeEntry entry, float value)
    {
        switch (entry.Member)
        {
            case FieldInfo field:
                field.SetValue(entry.Target, Convert.ChangeType(value, field.FieldType, CultureInfo.InvariantCulture));
                break;
            case PropertyInfo property:
                property.SetValue(entry.Target, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));
                break;
        }
    }

    private static bool TryConvertToFloat(object value, out float result)
    {
        try
        {
            result = Convert.ToSingle(value, CultureInfo.InvariantCulture);
            return true;
        }
        catch (Exception) when (value != null)
        {
            result = 0f;
            return false;
        }
    }

    private sealed record UpgradeEntry(object Target, MemberInfo Member, UpgradableAttribute Attribute);
}

public sealed record UpgradeInfo(
    string UpgradeId,
    float CurrentValue,
    float MaxValue,
    int Cost);
