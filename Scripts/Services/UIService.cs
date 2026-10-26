using Godot;
using System;

public partial class UIService : Node
{
    private EconomyService _economyService;
    private Label _scoreLabel;
    private Control _menuControl;
    private Control _inventoryControl;
    private Button _menuButton;
    private Texture2D MenuButtonIcon = GD.Load<Texture2D>("res://Art/UI/MenuButton.png");
    private Texture2D MenuButtonPressedIcon = GD.Load<Texture2D>("res://Art/UI/MenuButtonPressed.png");
    private Texture2D InventoryButtonIcon = GD.Load<Texture2D>("res://Art/UI/InventoryButton.png");
    private Texture2D InventoryButtonPressedIcon = GD.Load<Texture2D>("res://Art/UI/InventoryButtonPressed.png");
    private Boolean _isMenuButtonPressed = false;

    public override void _Ready()
    {
        _economyService = GetNode<EconomyService>("../EconomyService");
        _scoreLabel = GetNode<Label>("../../UI/Score");

        _economyService.ScoreChanged += OnScoreChanged;
        OnScoreChanged(_economyService.Score);

        _menuButton = GetNode<Button>("../../UI/BottomBar/MenuButton");
        _menuButton.Pressed += OnMenuButtonPress;
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
        if (GodotObject.IsInstanceValid(_inventoryControl))
        {
            _inventoryControl.QueueFree();
            _inventoryControl = null;
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

            if (GodotObject.IsInstanceValid(_menuControl))
            {
                _menuControl.QueueFree();
                _menuControl = null;
            }

            if (GodotObject.IsInstanceValid(_inventoryControl))
            {
                _inventoryControl.QueueFree();
                _inventoryControl = null;
            }
            
            _menuButton.Icon = MenuButtonIcon;
            _isMenuButtonPressed = false;
        }
    }

    private void OnInventoryButtonPress()
    {
        if (GodotObject.IsInstanceValid(_inventoryControl))
            return;

        var inventoryMenu = GD.Load<PackedScene>("res://Scenes/InventoryMenu.tscn");
        if (inventoryMenu == null)
        {
            GD.PrintErr("Failed to load inventory menu scene.");
            return;
        }

        var inventoryControl = inventoryMenu.Instantiate<Control>();
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.QueueFree();
            _menuControl = null;
        }

        _inventoryControl = inventoryControl;
        GetNode<Node2D>("../../UI").AddChild(_inventoryControl);
    }

    private void ShowMenu()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
            return;

        var menu = GD.Load<PackedScene>("res://Scenes/Menu.tscn");
        if (menu == null)
        {
            GD.PrintErr("Failed to load menu scene.");
            return;
        }

        _menuControl = menu.Instantiate<Control>();
        GetNode<Node2D>("../../UI").AddChild(_menuControl);
        var inventoryButton = _menuControl.GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/Inventory");
        inventoryButton.ButtonDown += OnInventoryButtonDown;
        inventoryButton.ButtonUp += OnInventoryButtonUp;
        inventoryButton.Pressed += OnInventoryButtonPress;
        _menuButton.Icon = MenuButtonPressedIcon;
        _isMenuButtonPressed = true;
    }

    private void OnInventoryButtonDown()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/Inventory").Icon = InventoryButtonPressedIcon;
        }
    }

    private void OnInventoryButtonUp()
    {
        if (GodotObject.IsInstanceValid(_menuControl))
        {
            _menuControl.GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/Inventory").Icon = InventoryButtonIcon;
        }
    }
}