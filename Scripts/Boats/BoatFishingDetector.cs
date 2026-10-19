using Godot;
using Incremental.Application.Interfaces;
using Incremental.Application.Models;
using Incremental.Game.Scripts.Fish;
using Incremental.Scripts.Boats;
using NumericsVector2 = System.Numerics.Vector2;

namespace Incremental.Game.Scripts.Boats;

public partial class BoatFishingDetector : Node
{
    private IFishDetectionService _detectionService = null!;
    private FishSpawner _fishSpawner = null!;
    private BoatController _boat = null!;

    [Export]
    public float FishingRadius { get; set; } = 100f;

    public int FishInRangeCount { get; private set; }

    public void Initialize(IFishDetectionService detectionService, FishSpawner fishSpawner, BoatController boat)
    {
        _detectionService = detectionService;
        _fishSpawner = fishSpawner;
        _boat = boat;
    }

    public override void _Process(double delta)
    {
        if (_detectionService == null || _fishSpawner == null || _boat == null)
        {
            return;
        }

        NumericsVector2 boatPosition = new(_boat.GlobalPosition.X, _boat.GlobalPosition.Y);

        IReadOnlyList<FishSpawn> nearbyFish = _detectionService.GetFishInRange(boatPosition, FishingRadius, _fishSpawner.Fish);

        FishInRangeCount = nearbyFish.Count;
    }
}
