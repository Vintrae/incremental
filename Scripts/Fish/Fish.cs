using Godot;
using System;

public partial class Fish : Node2D
{ 
	private enum FishType
	{
		Basic,
		Shiny
	}
	private FishType _fishType;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random r = new Random();
		Position = new Vector2(r.Next(0,720), r.Next(0,1280)); //TODO: Get size rather than hardcode

		_fishType = r.Next(0,100) <=90 ? _fishType = FishType.Basic : _fishType = FishType.Shiny;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
