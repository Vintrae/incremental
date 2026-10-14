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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_destination = Position;
        _fishingArea = GetNode<Area2D>("FishingArea");

        _fishingArea.AreaEntered += OnFishingAreaAreaEntered;
        _fishingArea.AreaExited += OnFishingAreaAreaExited;
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

    private void OnFishingAreaAreaEntered(Area2D area)
    {
        GD.Print("Ship entered fishing area: " + area.Name);
        // You can add logic here to start fishing or trigger events
    }

    private void OnFishingAreaAreaExited(Area2D area)
    {
        GD.Print("Ship exited fishing area: " + area.Name);
        // You can add logic here to stop fishing or trigger events
    }   

}
