using System;

namespace Incremental.Game.Scripts.Upgrades;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class UpgradableAttribute : Attribute
{
    public string UpgradeId { get; }
    public float MaxValue { get; }
    public int Cost { get; }

    public UpgradableAttribute(string upgradeId, float maxValue, int cost)
    {
        UpgradeId = upgradeId;
        MaxValue = maxValue;
        Cost = cost;
    }
}
