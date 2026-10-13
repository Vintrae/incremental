// WeightedRandom.cs
using System;
using System.Collections.Generic;
using System.Linq;

public static class WeightedRandom
{
    // Generic version where caller provides a weight selector
    public static Dictionary<T, float> CalculatePercentages<T>(IEnumerable<T> items, Func<T, int> weightSelector)
    {
        if (items == null) return new Dictionary<T, float>();

        var list = items.ToList();
        if (list.Count == 0) return new Dictionary<T, float>();

        var weights = list.Select(i => Math.Max(0, weightSelector(i))).ToList();
        double total = weights.Sum();

        var result = new Dictionary<T, float>();

        // If weights are negative or zero, assign equal percentages to all items
        if (total <= 0)
        {
            float equal = (float)Math.Round(100.0 / list.Count, 2);
            for (int i = 0; i < list.Count; i++) result[list[i]] = equal;

            // Assign the remaining difference to the first item to compensate for rounding errors
            float sum = result.Values.Sum();
            float diff = (float)Math.Round(100.0f - sum, 2);
            if (Math.Abs(diff) > 0.0001f)
            {
                var first = list[0];
                result[first] = result[first] + diff;
            }

            return result;
        }

        // Calculate percentages, round them and add rounding difference to the 
        // biggest item so they add up to 100%.
        var rawPercentages = weights.Select(w => (w / total) * 100.0).ToList();
        var rounded = rawPercentages.Select(p => (float)Math.Round(p, 2)).ToList();

        for (int i = 0; i < list.Count; i++)
            result[list[i]] = rounded[i];

        float sumRounded = result.Values.Sum();
        float diffTotal = (float)Math.Round(100.0f - sumRounded, 2);
        if (Math.Abs(diffTotal) > 0.0001f)
        {
            int maxIdx = 0;
            double maxRaw = rawPercentages[0];
            for (int i = 1; i < rawPercentages.Count; i++) 
                if (rawPercentages[i] > maxRaw) { 
                    maxRaw = rawPercentages[i]; 
                    maxIdx = i; 
                }

            var key = list[maxIdx];
            result[key] = (float)Math.Round(result[key] + diffTotal, 2);
        }

        return result;
    }

    // Convenience overload for items that implement ISpawnable
    public static Dictionary<T, float> CalculatePercentages<T>(IEnumerable<T> items) where T : ISpawnable
    {
        return CalculatePercentages(items, i => i.SpawnWeight);
    }

    /// <summary>
    /// Pick an item from the collection according to their weights by using the Weighted Random Selection algorithm..
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="weightSelector"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static T PickByWeight<T>(IEnumerable<T> items, Func<T, int> weightSelector, RandomUtil rng = null)
    {
        if (items == null) return default;

        var list = items.ToList();
        if (list.Count == 0) return default;

        var weights = list.Select(i => Math.Max(0, weightSelector(i))).ToList();
        double total = weights.Sum();
        if (total <= 0) return default;

        rng ??= new RandomUtil();
        float roll = rng.Range(0f, (float)total);
        double accum = 0.0;
        for (int i = 0; i < list.Count; i++)
        {
            accum += weights[i];
            if (roll <= accum) return list[i];
        }

        return list.Last();
    }

    // Convenience overload for ISpawnable
    public static T PickByWeight<T>(IEnumerable<T> items, RandomUtil rng = null) where T : ISpawnable
    {
        return PickByWeight(items, i => i.SpawnWeight, rng);
    }

    /// <summary>
    /// Pick an item from the collection according to their percentages by using the Weighted Random Selection algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="percentages"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static T PickByPercentage<T>(Dictionary<T, float> percentages, RandomUtil rng = null)
    {
        if (percentages == null || percentages.Count == 0) return default;

        rng ??= new RandomUtil();
        float roll = rng.Range(0f, 100f);
        float accum = 0f;
        foreach (var kv in percentages)
        {
            accum += kv.Value;
            if (roll <= accum) return kv.Key;
        }

        return percentages.Keys.Last();
    }
}
