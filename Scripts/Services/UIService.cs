using Godot;

public partial class UIService : Node
{
    private EconomyService _economyService;
    private Label _scoreLabel;

    public override void _Ready()
    {
        _economyService = GetNode<EconomyService>("../EconomyService");
        _scoreLabel = GetNode<Label>("../../UI/Score");

        _economyService.ScoreChanged += OnScoreChanged;
        OnScoreChanged(_economyService.Score);
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
}