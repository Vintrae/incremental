using Incremental.Game.Scripts.Interfaces;
using Incremental.Game.Scripts.Utils;
using Xunit;

public class WeightedRandomTests
{
    [Fact]
    public void CalculatePercentages_UsesRelativeWeightsAndTotalsOneHundred()
    {
        var items = new[]
        {
            new WeightedItem("common", 1),
            new WeightedItem("uncommon", 2),
            new WeightedItem("rare", 3)
        };

        var percentages = WeightedRandom.CalculatePercentages(items);

        Assert.Equal(16.67f, percentages[items[0]]);
        Assert.Equal(33.33f, percentages[items[1]]);
        Assert.Equal(50f, percentages[items[2]]);
        Assert.Equal(100f, percentages.Values.Sum(), 2);
    }

    [Fact]
    public void CalculatePercentages_ClampsNegativeWeightsToZero()
    {
        var items = new[]
        {
            new WeightedItem("negative", -10),
            new WeightedItem("positive", 5)
        };

        var percentages = WeightedRandom.CalculatePercentages(items);

        Assert.Equal(0f, percentages[items[0]]);
        Assert.Equal(100f, percentages[items[1]]);
    }

    [Fact]
    public void CalculatePercentages_DistributesEvenlyWhenAllWeightsAreZero()
    {
        var items = new[]
        {
            new WeightedItem("first", 0),
            new WeightedItem("second", 0),
            new WeightedItem("third", 0)
        };

        var percentages = WeightedRandom.CalculatePercentages(items);

        Assert.Equal(33.34f, percentages[items[0]]);
        Assert.Equal(33.33f, percentages[items[1]]);
        Assert.Equal(33.33f, percentages[items[2]]);
        Assert.Equal(100f, percentages.Values.Sum(), 2);
    }

    [Fact]
    public void CalculatePercentages_ReturnsEmptyResultForNullOrEmptyInput()
    {
        Assert.Empty(WeightedRandom.CalculatePercentages<WeightedItem>(null));
        Assert.Empty(WeightedRandom.CalculatePercentages(Array.Empty<WeightedItem>()));
    }

    [Fact]
    public void PickByWeight_ReturnsSameResultForSameSeed()
    {
        var items = new[]
        {
            new WeightedItem("common", 1),
            new WeightedItem("rare", 9)
        };

        var first = WeightedRandom.PickByWeight(items, new RandomUtil(42));
        var second = WeightedRandom.PickByWeight(items, new RandomUtil(42));

        Assert.Equal(first, second);
    }

    [Fact]
    public void PickByWeight_ReturnsDefaultWhenNoPositiveWeightExists()
    {
        var items = new[]
        {
            new WeightedItem("negative", -1),
            new WeightedItem("zero", 0)
        };

        Assert.Null(WeightedRandom.PickByWeight(items));
    }

    private sealed record WeightedItem(string Name, int SpawnWeight) : ISpawnable;
}
