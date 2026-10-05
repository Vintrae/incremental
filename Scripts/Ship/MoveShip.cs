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

	public override void _Input(InputEvent @event)
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

        Position = Position.MoveToward(_destination, Speed * (float)delta);
    }
}
