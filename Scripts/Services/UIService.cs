using Godot;
using System;

public partial class UIService : Node
{
    private EconomyService _economyService;
    private Label _scoreLabel;
    private Control _menuControl;
    private Button _menuButton;
    private Texture2D MenuButtonIcon = GD.Load<Texture2D>("res://Art/UI/MenuButton.png");
    private Texture2D MenuButtonPressedIcon = GD.Load<Texture2D>("res://Art/UI/MenuButtonPressed.png");
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
        if (!_isMenuButtonPressed)
        {
            GD.Print("Menu button pressed. Loading menu scene...");

            var menu = GD.Load<PackedScene>("res://Scenes/Menu.tscn");
            _menuControl = menu.Instantiate<Control>();

            GetTree().CurrentScene.AddChild(_menuControl);
            _menuButton.Icon = MenuButtonPressedIcon;
            _isMenuButtonPressed = true;
        }
        else
        {
            GD.Print("Menu button pressed again. Removing menu...");

            if (GodotObject.IsInstanceValid(_menuControl))
            {
                _menuControl.QueueFree();
                _menuControl = null;
            }
            
            _menuButton.Icon = MenuButtonIcon;
            _isMenuButtonPressed = false;
        }
    }
}