using Godot;
using System;

public partial class EconomyService : Node
{
    private FishingService _fishingService;

    public double Score { get; private set; }
    public event Action<double> ScoreChanged;

    public override void _Ready()
    {
        _fishingService = GetNode<FishingService>("../FishingService");
        _fishingService.EntityCaught += OnEntityCaught;
    }

    private void OnEntityCaught(Node entity)
    {
        if (entity is IValuable valuable)
        {
            Score += valuable.BaseValue;
            ScoreChanged?.Invoke(Score);
        }
    }
}