using Godot;

public partial class GameDirector : Node
{
    // Instantiate director classes.
    public static GameDirector Instance { get; private set; }
    public static FishDirector Fish { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        Fish = new FishDirector();
        AddChild(Fish);
    } 
}