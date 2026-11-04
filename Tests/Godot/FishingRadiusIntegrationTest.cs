#nullable enable

using System;
using Godot;
using Incremental.Game.Scripts.Services;
using Incremental.Game.Scripts.Ship;
using Incremental.Game.Scripts.Upgrades;

public partial class FishingRadiusIntegrationTest : Node
{
    public override void _Ready()
    {
        var gameScene = GD.Load<PackedScene>("res://Game/Scenes/incremental.tscn");
        if (gameScene == null)
        {
            Fail("Could not load the game scene.");
            return;
        }

        Node game = gameScene.Instantiate();
        AddChild(game);

        var ship = game.GetNode<Ship>("Entities/Ship");
        var fishingShape = ship.GetNode<CollisionShape2D>("FishingArea/FishingRadius")
            .Shape as CircleShape2D;
        if (fishingShape == null)
        {
            Fail("The ship's fishing area does not use a circle shape.");
            return;
        }

        if (!Mathf.IsEqualApprox(fishingShape.Radius, ship.FishingRadius))
        {
            Fail("The initial fishing collision radius does not match the ship attribute.");
            return;
        }

        var economy = game.GetNode<EconomyService>("Services/EconomyService");
        economy.AddScore(10d);

        var upgradeMenuScene = GD.Load<PackedScene>("res://Game/Scenes/UpgradeMenu.tscn");
        if (upgradeMenuScene == null)
        {
            Fail("Could not load the upgrades menu scene.");
            return;
        }

        Control upgradeMenu = upgradeMenuScene.Instantiate<Control>();
        game.GetNode<Node2D>("UI").AddChild(upgradeMenu);

        Label? fishingRadiusLabel = FindDescendant<Label>(
            upgradeMenu,
            label => label.Text == "Fishing Radius");
        if (fishingRadiusLabel == null)
        {
            Fail("The upgrades menu does not contain a Fishing Radius entry.");
            return;
        }

        Node upgradeRow = fishingRadiusLabel.GetParent().GetParent();
        Button? purchaseButton = FindDescendant<Button>(upgradeRow, _ => true);
        if (purchaseButton == null || purchaseButton.Disabled)
        {
            Fail("The Fishing Radius upgrade is not available to purchase.");
            return;
        }

        purchaseButton.EmitSignal(BaseButton.SignalName.Pressed);

        if (!Mathf.IsEqualApprox(ship.FishingRadius, 63.08484f))
        {
            Fail($"The upgrade did not change the ship radius as expected: {ship.FishingRadius}.");
            return;
        }

        if (!Mathf.IsEqualApprox(fishingShape.Radius, ship.FishingRadius))
        {
            Fail("Purchasing the upgrade did not update the fishing collision radius.");
            return;
        }

        if (purchaseButton.Text != "Buy (20 Score)")
        {
            Fail($"The upgrades menu did not refresh after purchase: {purchaseButton.Text}.");
            return;
        }

        upgradeMenu.QueueFree();

        GD.Print("Fishing radius integration test passed.");
        GetTree().Quit();
    }

    private void Fail(string message)
    {
        GD.PushError($"Fishing radius integration test failed: {message}");
        GetTree().Quit(1);
    }

    private static T? FindDescendant<T>(Node node, Func<T, bool> predicate) where T : Node
    {
        if (node is T match && predicate(match))
            return match;

        foreach (Node child in node.GetChildren())
        {
            T? found = FindDescendant(child, predicate);
            if (found != null)
                return found;
        }

        return null;
    }
}
