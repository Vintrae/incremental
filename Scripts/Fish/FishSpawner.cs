// FishSpawner.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class FishSpawner
{
    // Thin wrapper over the generic WeightedRandom utility so existing fish code keeps working.
    public static Dictionary<FishData, float> CalculateSpawnPercentages(IEnumerable<FishData> fishes)
    {
        return WeightedRandom.CalculatePercentages(fishes);
    }

    /// <summary>
    /// Pick a FishData from the collection according to their SpawnWeight.
    /// If a Godot RandomNumberGenerator is provided it will be used; otherwise System.Random is used.
    /// Returns null if collection is empty or all weights are <= 0.
    /// </summary>
    public static FishData PickByWeight(IEnumerable<FishData> fishes, RandomNumberGenerator rng = null)
    {
        if (fishes == null) return null;

        var list = fishes.Where(f => f != null).ToList();
        if (list.Count == 0) return null;

        // If caller provided a Godot RNG, use it to preserve deterministic behavior in-game.
        if (rng != null)
        {
            var weights = list.Select(f => Math.Max(0, f.SpawnWeight)).ToList();
            double total = weights.Sum();
            if (total <= 0) return null;

            rng.Randomize();
            double roll = rng.RandfRange(0f, (float)total);
            double accum = 0.0;
            for (int i = 0; i < list.Count; i++)
            {
                accum += weights[i];
                if (roll <= accum) return list[i];
            }

            return list.Last();
        }

        // Otherwise use the generic implementation with RandomUtil
        return WeightedRandom.PickByWeight(list, f => Math.Max(0, f.SpawnWeight), new RandomUtil());
    }
}
