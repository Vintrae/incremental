using System;
using System.Collections.Generic;
using Godot;

public partial class SpawnService : Node
{
    [Export] private FishData[] fishPool;
    //[Export] private Resource[] treasurePool; // placeholder for other pools

    public Resource SpawnRandomEntity(SpawnCategory category)
    {
        switch (category)
        {
            case SpawnCategory.Fish:
                if (fishPool == null || fishPool.Length == 0) return null;
                var rng = new RandomNumberGenerator();
                rng.Randomize();
                return FishSpawner.PickByWeight(fishPool, rng);

            // Add other categories here
            default:
                return null;
        }
    }
}
