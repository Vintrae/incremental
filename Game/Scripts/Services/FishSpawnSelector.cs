using System;
using System.Collections.Generic;
using System.Linq;
using Incremental.Game.Scripts.Utils;

namespace Incremental.Game.Scripts.Services;

public static class FishSpawnSelector
{
    public static T Select<T>(
        IEnumerable<T> fishPool,
        Func<T, bool> isUnlocked,
        Func<T, int> getSpawnWeight,
        RandomUtil random = null)
    {
        if (fishPool == null) return default;

        var unlockedFish = fishPool.Where(isUnlocked).ToList();
        return WeightedRandom.PickByWeight(unlockedFish, getSpawnWeight, random);
    }
}
