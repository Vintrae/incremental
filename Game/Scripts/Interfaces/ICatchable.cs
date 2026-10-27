namespace Incremental.Game.Scripts.Interfaces;

public interface ICatchable
{
    // Float difficulty used for catch progress calculations. Must be non-negative.
    float CatchDifficulty { get; }
}