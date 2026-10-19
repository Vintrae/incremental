using Godot;
using Incremental.Application.Interfaces;
using NumericsVector2 = System.Numerics.Vector2;

namespace Incremental.Scripts.Boats;

public partial class BoatController : Node2D
{
    [Export]
    public float MovementSpeed { get; set; } = 150f;

    private IBoatMovementService _movementService = null!;

    private NumericsVector2 _destination;
    private bool _hasDestination;

    public void Initialize(IBoatMovementService movementService)
    {
        _movementService = movementService;
    }

    public override void _Ready()
    {
        _destination = ToNumerics(GlobalPosition);
    }

    public override void _Process(double delta)
    {
        if (_movementService == null || !_hasDestination)
            return;

        NumericsVector2 currentPosition = ToNumerics(GlobalPosition);
        NumericsVector2 nextPosition = _movementService.CalculateNextPosition(currentPosition, _destination, MovementSpeed, (float)delta);

        // Girar la proa hacia el destino.
        NumericsVector2 direction = _destination - currentPosition;

        if (direction.LengthSquared() > 0.001f)
        {
            Rotation = Mathf.Atan2(direction.Y, direction.X)
                + Mathf.Pi / 2f;
        }

        GlobalPosition = ToGodot(nextPosition);

        if (NumericsVector2.DistanceSquared(ToNumerics(GlobalPosition), _destination) < 0.001f)
        {
            GlobalPosition = ToGodot(_destination);
            _hasDestination = false;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Ratón: establecer destino al pulsar.
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            SetDestination(GetGlobalMousePosition());
        }

        // Ratón: actualizar destino mientras se arrastra.
        if (@event is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            SetDestination(GetGlobalMousePosition());
        }

        // Pantalla táctil: establecer destino al tocar.
        if (@event is InputEventScreenTouch touch && touch.Pressed)
        {
            SetDestination(ScreenToWorld(touch.Position));
        }

        // Pantalla táctil: actualizar destino durante el arrastre.
        if (@event is InputEventScreenDrag drag)
        {
            SetDestination(ScreenToWorld(drag.Position));
        }
    }

    private void SetDestination(Vector2 position)
    {
        _destination = ToNumerics(position);
        _hasDestination = true;
    }

    private Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return GetViewport().GetCanvasTransform().AffineInverse() * screenPosition;
    }

    private static NumericsVector2 ToNumerics(Vector2 value)
    {
        return new NumericsVector2(value.X, value.Y);
    }

    private static Vector2 ToGodot(NumericsVector2 value)
    {
        return new Vector2(value.X, value.Y);
    }
}