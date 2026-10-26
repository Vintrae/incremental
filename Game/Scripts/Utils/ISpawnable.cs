using System.Collections.Generic;

public interface ISpawnable
{
    // Integer weight used for spawn probability calculations. Must be non-negative.
    int SpawnWeight { get; }
}

// Add shorthand to ISpawnable collections for calculating percentages using WeightedRandom.CalculatePercentages.
public static class SpawnableExtensions
{
    public static Dictionary<T, float> Percentages<T>(this IEnumerable<T> items) where T : ISpawnable
    {
        return WeightedRandom.CalculatePercentages(items);
    }
}
