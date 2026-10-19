using Godot;
using Incremental.Application.Interfaces;
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
    }

    public override void _ExitTree()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }
}