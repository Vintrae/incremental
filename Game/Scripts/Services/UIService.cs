using Godot;
using System;

namespace Incremental.Game.Scripts.Services;

public partial class UIService : Node
{
    private EconomyService _economyService;
    private Label _scoreLabel;
    private Control _menuControl;
    private Control _inventoryControl;
    private Control _upgradeControl;
    private Button _menuButton;
    private Button _closeButton;
    private Texture2D MenuButtonIcon = GD.Load<Texture2D>("res://Game/Art/UI/MenuButton.png");
    private Texture2D MenuButtonPressedIcon = GD.Load<Texture2D>("res://Game/Art/UI/MenuButtonPressed.png");
    private Texture2D InventoryButtonIcon = GD.Load<Texture2D>("res://Game/Art/UI/InventoryButton.png");
    private Texture2D InventoryButtonPressedIcon = GD.Load<Texture2D>("res://Game/Art/UI/InventoryButtonPressed.png");
    private Texture2D CloseButtonIcon = GD.Load<Texture2D>("res://Game/Art/UI/CloseButton.png");
    private Texture2D CloseButtonPressedIcon = GD.Load<Texture2D>("res://Game/Art/UI/CloseButtonPressed.png");
    private Boolean _isMenuButtonPressed = false;

    public override void _Ready()
    {
        _economyService = GetNode<EconomyService>("../EconomyService");
        _scoreLabel = GetNode<Label>("../../UI/Score");

        _economyService.ScoreChanged += OnScoreChanged;
        OnScoreChanged(_economyService.Score);

        _menuButton = GetNode<Button>("../../UI/BottomBar/MenuButton");
        _menuButton.Pressed += OnMenuButtonPress;

        _closeButton = GetNode<Button>("../../UI/BottomBar/CloseButton");
        _closeButton.ButtonDown += OnCloseButtonDown;
        _closeButton.ButtonUp += OnCloseButtonUp;
        _closeButton.Pressed += CloseAllMenus;
    }

    private void OnScoreChanged(double score)
    {
        _scoreLabel.Text = $"Score: {score:0}";
    }

    public override void _ExitTree()
    {
        if (GodotObject.IsInstanceValid(_economyService))
            _economyService.ScoreChanged -= OnScoreChanged;
    }

    private void OnMenuButtonPress()
    {
        if (GodotObject.IsInstanceValid(_inventoryControl) ||
            GodotObject.IsInstanceValid(_upgradeControl))
        {
            ClosePanels();
            ShowMenu();
            return;
        }

        if (!_isMenuButtonPressed)
        {
            ShowMenu();
        }
        else
        {
            GD.Print("Menu button pressed again. Removing menu...");
            CloseAllMenus();
        }
    }

    private void OnInventoryButtonPress()
    {
        OpenPanel("res://Game/Scenes/InventoryMenu.tscn", isUpgradeMenu: false);
    }

    private void OnUpgradesButtonPress()
    {
        OpenPanel("res://Game/Scenes/UpgradeMenu.tscn", isUpgradeMenu: true);
    }

    private void OpenPanel(string scenePath, bool isUpgradeMenu)
    {
        if ((isUpgradeMenu && GodotObject.IsInstanceValid(_upgradeControl)) ||
            (!isUpgradeMenu && GodotObject.IsInstanceValid(_inventoryControl)))
            return;

        var panelScene = GD.Load<PackedScene>(scenePath);
        if (panelScene == null)
        {
            GD.PrintErr($"Failed to load menu scene: {scenePath}");
            return;
        }

        var panelControl = panelScene.Instantiate<Control>();
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.QueueFree();
            _menuControl = null;
        }

        ClosePanels();
        if (isUpgradeMenu)
            _upgradeControl = panelControl;
        else
            _inventoryControl = panelControl;

        GetNode<Node2D>("../../UI").AddChild(panelControl);
        EnsureCloseButton();
    }

    private void ClosePanels()
    {
        if (GodotObject.IsInstanceValid(_inventoryControl))
        {
            _inventoryControl.QueueFree();
            _inventoryControl = null;
        }

        if (GodotObject.IsInstanceValid(_upgradeControl))
        {
            _upgradeControl.QueueFree();
            _upgradeControl = null;
        }
    }

    private void ShowMenu()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
            return;

        var menu = GD.Load<PackedScene>("res://Game/Scenes/Menu.tscn");
        if (menu == null)
        {
            GD.PrintErr("Failed to load menu scene.");
            return;
        }

        _menuControl = menu.Instantiate<Control>();
        GetNode<Node2D>("../../UI").AddChild(_menuControl);
        var inventoryButton = _menuControl.GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/InventoryContainer/Inventory");
        inventoryButton.ButtonDown += OnInventoryButtonDown;
        inventoryButton.ButtonUp += OnInventoryButtonUp;
        inventoryButton.Pressed += OnInventoryButtonPress;
        var upgradesButton = _menuControl.GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/UpgradesContainer/Upgrades");
        upgradesButton.Pressed += OnUpgradesButtonPress;
        _menuButton.Icon = MenuButtonPressedIcon;
        _isMenuButtonPressed = true;
        EnsureCloseButton();
    }

    private void OnInventoryButtonDown()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/InventoryContainer/Inventory").Icon = InventoryButtonPressedIcon;
        }
    }

    private void OnInventoryButtonUp()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/InventoryContainer/Inventory").Icon = InventoryButtonIcon;
        }
    }

    private void EnsureCloseButton()
    {
        if (_closeButton.GetParent() == null)
            _menuButton.GetParent().AddChild(_closeButton);

        _closeButton.Icon = CloseButtonIcon;
        _closeButton.Visible = true;
    }

    private void OnCloseButtonDown()
    {
        if (GodotObject.IsInstanceValid(_closeButton))
            _closeButton.Icon = CloseButtonPressedIcon;
    }

    private void OnCloseButtonUp()
    {
        if (GodotObject.IsInstanceValid(_closeButton))
            _closeButton.Icon = CloseButtonIcon;
    }

    private void CloseAllMenus()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.QueueFree();
            _menuControl = null;
        }

        ClosePanels();
        _closeButton.Visible = false;
        _closeButton.GetParent()?.RemoveChild(_closeButton);

        _menuButton.Icon = MenuButtonIcon;
        _isMenuButtonPressed = false;
    }
}