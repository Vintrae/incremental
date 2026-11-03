using Godot;
using Incremental.Game.Scripts.Fish;
using Incremental.Game.Scripts.Interfaces;
using System;
using System.Collections.Generic;

namespace Incremental.Game.Scripts.Services;

public partial class FishingService : Node
{
	private readonly Dictionary<Node, float> _progressByEntity = new();
	private EconomyService _economyService;

	public event Action<Node> EntityCaught;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_economyService = GetNode<EconomyService>("../../Services/EconomyService");
		var fishingArea = GetNode<Area2D>("../../Entities/Ship/FishingArea");
		fishingArea.AreaEntered += OnFishingAreaEntered;
		fishingArea.AreaExited += OnFishingAreaExited;
	}

	private void OnFishingAreaEntered(Area2D area)
	{
		var entity = area.GetParent();
		if (entity is ICatchable && !_progressByEntity.ContainsKey(entity))
		{
			_progressByEntity.Add(entity, 0f);
			if (entity is FishNode fish)
				fish.StartCapture();
			GD.Print($"Started catching {entity}");
		}
	}

	private void OnFishingAreaExited(Area2D area)
	{
		var entity = area.GetParent();
		_progressByEntity.Remove(entity);
		if (entity is FishNode fish && GodotObject.IsInstanceValid(fish))
			fish.StopCapture();
		GD.Print($"Stopped catching {entity}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (var entity in new List<Node>(_progressByEntity.Keys))
		{
			if (!GodotObject.IsInstanceValid(entity) ||
				entity is not ICatchable catchable)
			{
				_progressByEntity.Remove(entity);
				continue;
			}

			if (catchable.CatchDifficulty <= 0f)
			{
				GD.Print($"Entity {entity} has invalid catch difficulty. Skipping.");
				continue;
			}

			float progress = _progressByEntity[entity];
			progress = CatchProgress.Advance(progress, (float)delta, catchable.CatchDifficulty);
			_progressByEntity[entity] = progress;
			if (entity is FishNode fish)
				fish.UpdateCaptureProgress(progress);

			if (progress >= 1f)
			{
				_progressByEntity.Remove(entity);
				if (entity is FishNode caughtFish)
					caughtFish.StopCapture();
				EntityCaught?.Invoke(entity);
				entity.QueueFree();
				GD.Print($"Caught {entity}");
			}
		}
	}
}
