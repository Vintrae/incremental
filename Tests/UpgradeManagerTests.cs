using Incremental.Game.Scripts.Upgrades;
using Xunit;

public class UpgradeManagerTests
{
    [Fact]
    public void RegisterAndUpgrade_UsesUpgradeId()
    {
        var target = new UpgradeTarget();
        var manager = new UpgradeManager();

        manager.Register(target);

        Assert.True(manager.TryGet(UpgradeIds.ShipSpeed, out UpgradeInfo info));
        Assert.Equal(100f, info.CurrentValue);
        Assert.Equal(1000f, info.MaxValue);
        Assert.Equal(10, info.Cost);
        Assert.True(manager.TryUpgrade(UpgradeIds.ShipSpeed, 50f));
        Assert.Equal(150f, target.Speed);
    }

    [Fact]
    public void Upgrade_DoesNotExceedMaximum()
    {
        var target = new UpgradeTarget { Speed = 980f };
        var manager = new UpgradeManager();
        manager.Register(target);

        Assert.True(manager.TryUpgrade(UpgradeIds.ShipSpeed, 50f));
        Assert.Equal(1000f, target.Speed);
        Assert.False(manager.TryUpgrade(UpgradeIds.ShipSpeed, 1f));
    }

    [Fact]
    public void Register_RejectsDuplicateUpgradeIds()
    {
        var manager = new UpgradeManager();
        manager.Register(new UpgradeTarget());

        Assert.Throws<InvalidOperationException>(() => manager.Register(new UpgradeTarget()));
    }

    private sealed class UpgradeTarget
    {
        [Upgradable(UpgradeIds.ShipSpeed, 1000f, 10)]
        public float Speed = 100f;
    }
}
