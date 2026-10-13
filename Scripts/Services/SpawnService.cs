using System;
using System.Collections.Generic;
using Godot;

public partial class SpawnService : Node
{
    [Export] private FishData[] fishPool;
    //[Export] private Resource[] treasurePool; // placeholder for other pools

    public Resource SpawnEntity(SpawnCategory category, string entityName)
    {
        switch (category)
        {
            case SpawnCategory.Fish:
                if (fishPool == null || fishPool.Length == 0) return null;
                FishData targetFish = System.Array.Find(fishPool, fish => fish.FishName.ToLower() == entityName.ToLower());
                if (!targetFish.IsUnlocked) return null;
                return null;


            // Add other categories here
            default:
                return null;
        }
    }
    
    public Resource SpawnRandomEntity(SpawnCategory category)
    {
        switch (category)
        {
            case SpawnCategory.Fish:
                if (fishPool == null || fishPool.Length == 0) return null;
                //var rng = new RandomUtil();
                return WeightedRandom.PickByWeight(fishPool);

            // Add other categories here
            default:
                return null;
        }
    }
}
