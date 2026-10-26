using System.IO;
using System.Linq;
using Xunit;

public class GameplayLogicTests
{
    [Fact]
    public void ResourceInventory_AddsResourcesAndAccumulatesAmounts()
    {
        string resourceDirectory = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "Game", "Resources", "Game"));
        string[] resourceNames = Directory
            .GetFiles(resourceDirectory, "*.tres", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .OfType<string>()
            .Distinct()
            .ToArray();
        Assert.NotEmpty(resourceNames);

        var inventory = new ResourceInventory();
        foreach (string resourceName in resourceNames)
            inventory.Register(resourceName);

        Assert.True(inventory.Add("cod", 2));
        Assert.True(inventory.Add("cod", 3));

        Assert.Equal(5, inventory.GetAmount("cod"));
        Assert.Equal(resourceNames.Length, inventory.ResourceTypeCount);
    }

    [Fact]
    public void ResourceInventory_RejectsUnknownOrNonPositiveAdds()
    {
        var inventory = CreateInventory();

        Assert.False(inventory.Add("unknown", 1));
        Assert.False(inventory.Add("cod", 0));
        Assert.False(inventory.Add("cod", -1));

        Assert.Equal(0, inventory.GetAmount("cod"));
    }

    [Fact]
    public void ResourceInventory_SpendsExactAndPartialAmounts()
    {
        var inventory = CreateInventory();
        inventory.Add("cod", 5);

        Assert.True(inventory.TrySpend("cod", 2));
        Assert.Equal(3, inventory.GetAmount("cod"));
        Assert.True(inventory.TrySpend("cod", 3));
        Assert.Equal(0, inventory.GetAmount("cod"));
    }

    [Fact]
    public void ResourceInventory_DoesNotSpendWhenUnknownInsufficientOrInvalid()
    {
        var inventory = CreateInventory();
        inventory.Add("cod", 2);

        Assert.False(inventory.TrySpend("unknown", 1));
        Assert.False(inventory.TrySpend("cod", 3));
        Assert.False(inventory.TrySpend("cod", 0));
        Assert.False(inventory.TrySpend("cod", -1));

        Assert.Equal(2, inventory.GetAmount("cod"));
    }

    [Fact]
    public void CatchProgress_AdvancesBasedOnCatchDifficulty()
    {
        float progress = CatchProgress.Advance(0.25f, 0.5f, 2f);

        Assert.Equal(0.5f, progress);
    }

    [Fact]
    public void CatchProgress_StopsAtComplete()
    {
        float progress = CatchProgress.Advance(0.8f, 1f, 2f);

        Assert.Equal(1f, progress);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    public void CatchProgress_DoesNotAdvanceForInvalidDifficulty(float difficulty)
    {
        float progress = CatchProgress.Advance(0.4f, 1f, difficulty);

        Assert.Equal(0.4f, progress);
    }

    [Fact]
    public void FishSpawnSelector_ChoosesOnlyUnlockedFish()
    {
        var lockedFish = new SpawnCandidate("locked", false, 100);
        var unlockedFish = new SpawnCandidate("unlocked", true, 1);
        var fishPool = new[] { lockedFish, unlockedFish };

        var selectedFish = FishSpawnSelector.Select(
            fishPool,
            fish => fish.IsUnlocked,
            fish => fish.SpawnWeight,
            new RandomUtil(42));

        Assert.Same(unlockedFish, selectedFish);
    }

    [Fact]
    public void FishSpawnSelector_ReturnsDefaultWhenNoFishCanSpawn()
    {
        var lockedFish = new SpawnCandidate("locked", false, 100);
        var zeroWeightFish = new SpawnCandidate("zero", true, 0);

        var selectedFish = FishSpawnSelector.Select(
            new[] { lockedFish, zeroWeightFish },
            fish => fish.IsUnlocked,
            fish => fish.SpawnWeight,
            new RandomUtil(42));

        Assert.Null(selectedFish);
    }

    [Fact]
    public void FishSpawnSelector_ReturnsDefaultForNullPool()
    {
        Assert.Null(FishSpawnSelector.Select<SpawnCandidate>(
            null,
            fish => fish.IsUnlocked,
            fish => fish.SpawnWeight));
    }

    private static ResourceInventory CreateInventory()
    {
        var inventory = new ResourceInventory();
        inventory.Register("cod");
        return inventory;
    }

    private sealed record SpawnCandidate(string Name, bool IsUnlocked, int SpawnWeight) : ISpawnable;
}
