using System;
using System.Collections.Generic;
using Godot;
using Incremental.Game.Scripts.Services;
using ShipNode = Incremental.Game.Scripts.Ship.Ship;

namespace Incremental.Game.Scripts.UI;

public partial class UpgradeMenu : Control
{
    private EconomyService _economyService;
    private UpgradeService _upgradeService;
    private readonly Dictionary<string, UpgradeRow> _rows = new(StringComparer.Ordinal);

    public override void _Ready()
    {
        _economyService = GetNode<EconomyService>("../../Services/EconomyService");
        _upgradeService = GetNode<ShipNode>("../../Entities/Ship").Upgrades;

        BuildUpgradeList();
        _economyService.ScoreChanged += OnScoreChanged;
        _upgradeService.UpgradeChanged += OnUpgradeChanged;
        RefreshRows();
    }

    public override void _ExitTree()
    {
        if (GodotObject.IsInstanceValid(_economyService))
            _economyService.ScoreChanged -= OnScoreChanged;
        if (_upgradeService != null)
            _upgradeService.UpgradeChanged -= OnUpgradeChanged;
    }

    private void BuildUpgradeList()
    {
        var content = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };
        GetNode<PanelContainer>("PanelContainer").AddChild(content);

        content.AddChild(new Label
        {
            Text = "Upgrades",
            HorizontalAlignment = HorizontalAlignment.Center
        });

        var margin = new MarginContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };
        margin.AddThemeConstantOverride("margin_left", 24);
        margin.AddThemeConstantOverride("margin_top", 24);
        margin.AddThemeConstantOverride("margin_right", 24);
        margin.AddThemeConstantOverride("margin_bottom", 24);
        content.AddChild(margin);

        var scrollContainer = new ScrollContainer
        {
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            VerticalScrollMode = ScrollContainer.ScrollMode.Auto,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };
        margin.AddChild(scrollContainer);

        var rows = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        rows.AddThemeConstantOverride("separation", 12);
        scrollContainer.AddChild(rows);

        foreach (UpgradeInfo info in _upgradeService.GetAll())
        {
            var panel = new PanelContainer
            {
                CustomMinimumSize = new Vector2(0, 96),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };
            rows.AddChild(panel);

            var rowMargin = new MarginContainer
            {
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                SizeFlagsVertical = SizeFlags.ExpandFill
            };
            rowMargin.AddThemeConstantOverride("margin_left", 16);
            rowMargin.AddThemeConstantOverride("margin_top", 8);
            rowMargin.AddThemeConstantOverride("margin_right", 16);
            rowMargin.AddThemeConstantOverride("margin_bottom", 8);
            panel.AddChild(rowMargin);

            var row = new HBoxContainer
            {
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                SizeFlagsVertical = SizeFlags.ExpandFill
            };
            row.AddThemeConstantOverride("separation", 16);
            rowMargin.AddChild(row);

            var description = new VBoxContainer
            {
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                SizeFlagsVertical = SizeFlags.ShrinkCenter
            };
            row.AddChild(description);
            description.AddChild(new Label { Text = info.DisplayName });
            description.AddChild(new Label
            {
                Text = $"+{info.IncreaseAmount:0.##} {info.AttributeId} per level"
            });

            var levelLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            row.AddChild(levelLabel);

            var purchaseButton = new Button
            {
                CustomMinimumSize = new Vector2(180, 56),
                SizeFlagsVertical = SizeFlags.ShrinkCenter
            };
            var upgradeId = info.UpgradeId;
            purchaseButton.Pressed += () => OnPurchasePressed(upgradeId);
            row.AddChild(purchaseButton);

            _rows.Add(upgradeId, new UpgradeRow(levelLabel, purchaseButton));
        }
    }

    private void RefreshRows()
    {
        foreach (UpgradeInfo info in _upgradeService.GetAll())
        {
            UpgradeRow row = _rows[info.UpgradeId];
            row.Level.Text = $"Level: {info.Level} / {info.MaxLevel}";

            if (info.NextCost is double nextCost)
            {
                row.Purchase.Text = $"Buy ({nextCost:0.##} Score)";
                row.Purchase.Disabled = _economyService.Score < nextCost;
            }
            else
            {
                row.Purchase.Text = "Max Level";
                row.Purchase.Disabled = true;
            }
        }
    }

    private void OnPurchasePressed(string upgradeId)
    {
        _upgradeService.TryPurchase(upgradeId, _economyService);
        RefreshRows();
    }

    private void OnScoreChanged(double score) => RefreshRows();

    private void OnUpgradeChanged() => RefreshRows();

    private sealed record UpgradeRow(Label Level, Button Purchase);
}
