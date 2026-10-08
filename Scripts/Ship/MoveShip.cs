using Godot;
using System;

public partial class MoveShip : Node2D
{
	private Vector2 _destination;
	private bool _hasDestination;

	[Export]
    public float Speed = 500f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_destination = Position;
	}

	public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch && touch.Pressed)
        {
            _destination = touch.Position;
            _hasDestination = true;
        }
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (!_hasDestination)
            return;

        float distance = Position.DistanceTo(_destination);

        if (distance < 2f)
        {
            Position = _destination;
            _hasDestination = false;
            return;
        }

        Vector2 direction = _destination - GlobalPosition;
        Rotation = direction.Angle() + Mathf.Pi / 2;
        Position = Position.MoveToward(_destination, Speed * (float)delta);
    }
}
