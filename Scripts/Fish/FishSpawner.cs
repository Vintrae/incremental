using Godot;
using Incremental.Application.Interfaces;
using Incremental.Application.Models;
using NumericsVector2 = System.Numerics.Vector2;

namespace Incremental.Game.Scripts.Fish;

public partial class FishSpawner : Node2D
{
    [Export]
    public PackedScene FishScene { get; set; } = null!;

    [Export]
    public int SpawnCount { get; set; } = 30;

    [Export]
    public Vector2 MapSize { get; set; } = new(1600f, 900f);

    private readonly List<FishSpawn> _fish = new();
    public IReadOnlyList<FishSpawn> Fish => _fish;

    private IFishSpawnService _fishSpawnService = null!;

    public void Initialize(IFishSpawnService fishSpawnService)
    {
        _fishSpawnService = fishSpawnService;
    }

    public void SpawnFish()
    {
        if (_fishSpawnService == null)
            throw new InvalidOperationException("FishSpawner no está inicializado.");

        if (FishScene == null)
            throw new InvalidOperationException("Debes asignar FishScene en el inspector.");

        _fish.Clear();

        var spawns = _fishSpawnService.Generate(SpawnCount, NumericsVector2.Zero, new NumericsVector2(MapSize.X, MapSize.Y));

        foreach (var spawn in spawns)
        {
            FishController fishNode = FishScene.Instantiate<FishController>();

            AddChild(fishNode);

            fishNode.Position = new Vector2(spawn.Position.X,spawn.Position.Y);

            fishNode.Initialize(spawn.Fish);

            _fish.Add(spawn);
        }
    }
}
