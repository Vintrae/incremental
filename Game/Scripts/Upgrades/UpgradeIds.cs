using System;
using System.Collections.Generic;

namespace Incremental.Game.Scripts.Upgrades;

public sealed class UpgradeDefinition
{
    public string Id { get; }
    public string DisplayName { get; }
    public string AttributeId { get; }
    public float IncreaseAmount { get; }
    public double BaseCost { get; }
    public int MaxLevel { get; }

    public UpgradeDefinition(
        string id,
        string displayName,
        string attributeId,
        float increaseAmount,
        double baseCost,
        int maxLevel = 1)
    {
        if (!UpgradeIdValidation.IsAlphanumeric(id))
            throw new ArgumentException("An upgrade ID must contain only letters and digits.", nameof(id));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("An upgrade needs a display name.", nameof(displayName));
        if (!UpgradeIdValidation.IsAlphanumeric(attributeId))
            throw new ArgumentException("An attribute ID must contain only letters and digits.", nameof(attributeId));
        if (!float.IsFinite(increaseAmount) || increaseAmount <= 0f)
            throw new ArgumentOutOfRangeException(nameof(increaseAmount), "The increase amount must be positive and finite.");
        if (!double.IsFinite(baseCost) || baseCost <= 0d)
            throw new ArgumentOutOfRangeException(nameof(baseCost), "The base cost must be positive and finite.");
        if (maxLevel <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLevel), "The maximum level must be positive.");

        Id = id;
        DisplayName = displayName;
        AttributeId = attributeId;
        IncreaseAmount = increaseAmount;
        BaseCost = baseCost;
        MaxLevel = maxLevel;
    }

    public double GetCostForLevel(int level)
    {
        if (level < 0 || level >= MaxLevel)
            throw new ArgumentOutOfRangeException(nameof(level), "The requested level must be available for purchase.");

        return BaseCost * Math.Pow(2d, level);
    }
}

public static class UpgradeIds
{
    public const string ShipSpeedBoost = "ShipSpeedBoost";

    public static IReadOnlyList<UpgradeDefinition> All { get; } = Array.AsReadOnly(new[]
    {
        new UpgradeDefinition(ShipSpeedBoost, "Ship Speed", AttributeIds.ShipSpeed, 50f, 5d, 10)
    });
}