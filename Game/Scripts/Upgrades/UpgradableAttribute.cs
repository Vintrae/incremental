using System;

namespace Incremental.Game.Scripts.Upgrades;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class UpgradableAttribute : Attribute
{
    public string AttributeId { get; }
    public float MaxValue { get; }

    public UpgradableAttribute(string attributeId, float maxValue)
    {
        if (!UpgradeIdValidation.IsAlphanumeric(attributeId))
            throw new ArgumentException("An attribute ID must contain only letters and digits.", nameof(attributeId));
        if (!float.IsFinite(maxValue) || maxValue <= 0f)
            throw new ArgumentOutOfRangeException(nameof(maxValue), "The maximum value must be positive and finite.");

        AttributeId = attributeId;
        MaxValue = maxValue;
    }
}
