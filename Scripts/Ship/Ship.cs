using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Ship : Node2D
{
    private Vector2 _destination;
    private bool _hasDestination;
    private Area2D _fishingArea;

    [Export]
    public float Speed = 500f;

    [Export]
    public int FPS = 30;
    private float interval = 0f;
    private float intervalMax = 0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _destination = Position;
        _fishingArea = GetNode<Area2D>("FishingArea");

        intervalMax = 1f / FPS;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch && touch.Pressed)
        {
            _destination = touch.Position;
            _hasDestination = true;
        }
        if (@event is InputEventScreenDrag drag)
        {
            _destination = (ScreenToWorld(drag.Position));
        }
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        interval += (float)delta;

        if (!_hasDestination)
            return;

        if (FPS == 0f || interval >= intervalMax)
        {
            float distance = Position.DistanceTo(_destination);

            if (distance < 2f)
            {
                Position = _destination;
                _hasDestination = false;
                return;
            }

            Vector2 direction = _destination - GlobalPosition;
            Rotation = direction.Angle() + Mathf.Pi / 2;
            interval = 0f;
        }

        Position = Position.MoveToward(_destination, Speed * (float)delta);
    }
    private Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return GetViewport().GetCanvasTransform().AffineInverse() * screenPosition;
    }
}
