using System;
using System.Collections.Generic;

namespace Incremental.Game.Scripts.Upgrades;

public static class AttributeIds
{
    public const string ShipSpeed = "ShipSpeed";
    public const string FishingRadius = "FishingRadius";

    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
    {
        ShipSpeed,
        FishingRadius
    });
}

internal static class UpgradeIdValidation
{
    public static bool IsAlphanumeric(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        foreach (char character in id)
        {
            if (!char.IsAsciiLetterOrDigit(character))
                return false;
        }

        return true;
    }
}
