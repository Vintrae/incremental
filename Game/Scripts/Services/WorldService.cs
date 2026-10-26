using Godot;
using System;

public partial class WorldService : Node2D
{
	private AnimatedSprite2D _waterAnimation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_waterAnimation = GetNode<AnimatedSprite2D>("../../Water/WaterAnimation");
		_waterAnimation.Play("Sea");
		FillViewportWithWater();
	}

	private void FillViewportWithWater()
	{
		var waterParent = _waterAnimation.GetParent();
		var frameSize = _waterAnimation.SpriteFrames.GetFrameTexture(_waterAnimation.Animation, 0).GetSize();
		var tileSize = frameSize * _waterAnimation.Scale;
		var viewportSize = GetViewportRect().Size;
		var columns = Mathf.CeilToInt(viewportSize.X / tileSize.X);
		var rows = Mathf.CeilToInt(viewportSize.Y / tileSize.Y);
		var gridSize = new Vector2(columns * tileSize.X, rows * tileSize.Y);
		var gridOrigin = (viewportSize - gridSize) / 2.0f;

		for (var row = 0; row < rows; row++)
		{
			for (var column = 0; column < columns; column++)
			{
				var tile = row == 0 && column == 0 ? _waterAnimation : (AnimatedSprite2D)_waterAnimation.Duplicate();

				if (tile != _waterAnimation)
				{
					waterParent.AddChild(tile);
					tile.Play("Sea");
				}

				tile.Position = gridOrigin + new Vector2(
					(column + 0.5f) * tileSize.X,
					(row + 0.5f) * tileSize.Y);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
