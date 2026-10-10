// RandomUtil.cs
using System;

public class RandomUtil
{
    private readonly Random _rng;

    public RandomUtil(int? seed = null)
    {
        _rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>
    /// Returns a float in range [0,1).
    /// </summary>
    public float NextFloat01()
    {
        return (float)_rng.NextDouble();
    }

    /// <summary>
    /// Returns a float in range [min, max).
    /// </summary>
    public float Range(float min, float max)
    {
        if (min >= max) return min;
        return min + NextFloat01() * (max - min);
    }
}
