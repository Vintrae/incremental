// FishData.cs
using Godot;
using Incremental.Game.Scripts.Interfaces;

namespace Incremental.Game.Scripts.Fish;

public enum BiomeType
{
    ShallowWater,
    DeepSea,
    VolcanicVent,
    ArcticWaters
}

[GlobalClass]
public partial class FishData : Resource, ISpawnable, ICatchable, IInventoryResourceDefinition
{
    [Export] public string FishName { get; set; } = "Goldfish";
    [Export] public double BaseValue { get; set; } = 10.0;
    [Export] public float CatchDifficulty { get; set; } = 1.0f;
    [Export] public Texture2D Icon { get; set; }
    public Texture2D InventoryIcon => Icon;

    [ExportGroup("Spawn Rules")]
    [Export] public int SpawnWeight { get; set; } = 100; // Lower weight = rarer
    [Export] public bool IsUnlocked { get; set; } = false;
    [Export] public BiomeType AllowedBiome { get; set; } = BiomeType.ShallowWater;
}