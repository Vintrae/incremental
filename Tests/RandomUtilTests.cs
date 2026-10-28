using Xunit;
using Incremental.Game.Scripts.Utils;

public class RandomUtilTests
{
    [Fact]
    public void Range_ReturnsMinimumWhenRangeIsInvalid()
    {
        var random = new RandomUtil(42);

        Assert.Equal(5f, random.Range(5f, 5f));
        Assert.Equal(10f, random.Range(10f, 5f));
    }

    [Fact]
    public void Range_StaysWithinRequestedBounds()
    {
        var random = new RandomUtil(42);

        for (int i = 0; i < 100; i++)
        {
            float value = random.Range(-2f, 7f);

            Assert.InRange(value, -2f, 7f);
        }
    }

    [Fact]
    public void SeededInstances_ProduceTheSameSequence()
    {
        var first = new RandomUtil(42);
        var second = new RandomUtil(42);

        for (int i = 0; i < 10; i++)
            Assert.Equal(first.NextFloat01(), second.NextFloat01());
    }
}
