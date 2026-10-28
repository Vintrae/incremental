using Incremental.Game.Scripts.Services;
using Incremental.Game.Scripts.Upgrades;
using Xunit;

public class UpgradeManagerTests
{
    [Fact]
    public void RegisterAndUpgrade_UsesUpgradeDefinition()
    {
        var target = new UpgradeTarget();
        var manager = new UpgradeService();
        var economy = new TestScoreWallet(100);

        manager.Register(target);

        Assert.True(manager.TryGet(UpgradeIds.ShipSpeedBoost, out UpgradeInfo info));
        Assert.Equal(AttributeIds.ShipSpeed, info.AttributeId);
        Assert.Equal("Ship Speed", info.DisplayName);
        Assert.Equal(100f, info.CurrentValue);
        Assert.Equal(1000f, info.MaxValue);
        Assert.Equal(50f, info.IncreaseAmount);
        Assert.Equal(0, info.Level);
        Assert.Equal(10, info.MaxLevel);
        Assert.Equal(5d, info.NextCost);
        Assert.True(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.Equal(150f, target.Speed);
        Assert.True(manager.TryGet(UpgradeIds.ShipSpeedBoost, out info));
        Assert.Equal(1, info.Level);
        Assert.Equal(10d, info.NextCost);
        Assert.Equal(95d, economy.Score);
    }

    [Fact]
    public void Upgrade_DoesNotExceedMaximum()
    {
        var target = new UpgradeTarget { Speed = 980f };
        var manager = new UpgradeService();
        var economy = new TestScoreWallet(100);
        manager.Register(target);

        Assert.True(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.Equal(1000f, target.Speed);
        Assert.False(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
    }

    [Fact]
    public void Upgrade_DoesNotExceedMaximumLevel()
    {
        var target = new UpgradeTarget();
        var manager = new UpgradeService();
        var economy = new TestScoreWallet(10000);
        manager.Register(target);

        Assert.True(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.True(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.Equal(200f, target.Speed);
        for (int i = 0; i < 8; i++)
            Assert.True(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.False(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.True(manager.TryGet(UpgradeIds.ShipSpeedBoost, out UpgradeInfo info));
        Assert.Equal(info.MaxLevel, info.Level);
        Assert.Null(info.NextCost);
    }

    [Fact]
    public void Purchase_RequiresEnoughScore()
    {
        var target = new UpgradeTarget();
        var manager = new UpgradeService();
        var economy = new TestScoreWallet(4);
        manager.Register(target);

        Assert.False(manager.TryPurchase(UpgradeIds.ShipSpeedBoost, economy));
        Assert.Equal(4d, economy.Score);
        Assert.Equal(100f, target.Speed);
    }

    [Fact]
    public void Register_RejectsDuplicateUpgradeIds()
    {
        var manager = new UpgradeService();
        manager.Register(new UpgradeTarget());

        Assert.Throws<InvalidOperationException>(() => manager.Register(new UpgradeTarget()));
    }

    private sealed class UpgradeTarget
    {
        [Upgradable(AttributeIds.ShipSpeed, 1000f)]
        public float Speed = 100f;
    }

    private sealed class TestScoreWallet(double score) : IScoreWallet
    {
        public double Score { get; private set; } = score;

        public bool TrySpendScore(double amount)
        {
            if (amount <= 0d || Score < amount)
                return false;

            Score -= amount;
            return true;
        }
    }
}
