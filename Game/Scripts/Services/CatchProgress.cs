namespace Incremental.Game.Scripts.Services;

public static class CatchProgress
{
    public static float Advance(float progress, float delta, float catchDifficulty)
    {
        if (catchDifficulty <= 0f)
            return progress;

        return System.Math.Min(1f, progress + delta / catchDifficulty);
    }
}
