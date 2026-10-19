using Godot;
using Incremental.Application.Interfaces;
using Incremental.Game.Scripts.Boats;
using Incremental.Game.Scripts.Fish;
using Incremental.Scripts.Boats;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Game.Bootstrap;

public partial class GameBootstrap : Node2D
{
    private ServiceProvider? _serviceProvider;

    public override void _Ready()
    {
        _serviceProvider = DependencyInjection.Build();

        BoatController boat = GetNode<BoatController>("Boat");

        boat.Initialize(_serviceProvider.GetRequiredService<IBoatMovementService>());

        FishSpawner fishSpawner = GetNode<FishSpawner>("FishSpawner");

        fishSpawner.Initialize(_serviceProvider.GetRequiredService<IFishSpawnService>());

        BoatFishingDetector detector = GetNode<BoatFishingDetector>("BoatFishingDetector");

        detector.Initialize(_serviceProvider.GetRequiredService<IFishDetectionService>(), fishSpawner, boat);

        fishSpawner.SpawnFish();
    }

    public override void _ExitTree()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }
}