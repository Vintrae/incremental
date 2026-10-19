using Godot;
using Incremental.Application.Interfaces;
using Incremental.Application.Models;
using Incremental.Game.Scripts.Fish;
using Incremental.Scripts.Boats;
using NumericsVector2 = System.Numerics.Vector2;

namespace Incremental.Game.Scripts.Boats;

public partial class BoatFishingController : Node
{
    [Export]
    public float CaptureDurationSeconds { get; set; } = 3f;

    [Export]
    public float FishingRadius { get; set; } = 100f;

    private IFishCaptureService _captureService = null!;
    private IFishDetectionService _detectionService = null!;
    private FishSpawner _fishSpawner = null!;
    private BoatController _boat = null!;

    public void Initialize(IFishCaptureService captureService, IFishDetectionService detectionService, FishSpawner fishSpawner, BoatController boat)
    {
        _captureService = captureService;
        _detectionService = detectionService;
        _fishSpawner = fishSpawner;
        _boat = boat;
    }

    public override void _Process(double delta)
    {
        if (_captureService == null || _detectionService == null || _fishSpawner == null || _boat == null)
            return;

        NumericsVector2 boatPosition = new(_boat.GlobalPosition.X, _boat.GlobalPosition.Y);
        IReadOnlyList<FishSpawn> nearbyFish = _detectionService.GetFishInRange(boatPosition, FishingRadius, _fishSpawner.Fish);
        FishCaptureProgress? current = _captureService.CurrentCapture;

        if (current != null)
        {
            bool stillInRange = nearbyFish.Any(spawn => spawn.Fish.Id == current.Fish.Id);

            if (!stillInRange)
            {
                _captureService.CancelCapture();
                return;
            }

            _captureService.Advance((float)delta);

            Domain.Fish.Fish? captured = _captureService.CompleteCapture();

            if (captured != null)
            {
                FishController? fishNode =
                    _fishSpawner.GetChildren()
                        .OfType<FishController>()
                        .FirstOrDefault(node =>
                            node.Fish.Id == captured.Id);

                if (fishNode != null)
                    _fishSpawner.RemoveFish(fishNode);
            }

            return;
        }

        FishSpawn? candidate = nearbyFish.FirstOrDefault(spawn => spawn.Fish.State == Domain.Fish.FishCaptureState.Available);

        if (candidate != null)
            _captureService.TryStartCapture(candidate.Fish, CaptureDurationSeconds);
    }
}
