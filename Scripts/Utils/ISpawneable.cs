// ISpawneable.cs
public interface ISpawneable
{
    // Integer weight used for spawn probability calculations. Must be non-negative.
    int SpawnWeight { get; }
}
