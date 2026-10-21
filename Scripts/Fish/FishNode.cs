// FishNode.cs
using Godot;

public partial class FishNode : Sprite2D, ICatchable, IValuable
{
    private FishData _data;

    public FishData Data => _data;
    public double BaseValue => _data?.BaseValue ?? 0.0;
    public float CatchDifficulty => _data?.CatchDifficulty ?? 1.0f;

    // This method initializes the node with the specific FishData (e.g., "cod")
    public void Initialize(FishData fishData)
    {
        _data = fishData;

        // Apply visual properties directly from the resource
        Texture = _data.Icon;
        
        // If you have a Label child node for the name, you can set it too:
        // GetNode<Label>("NameLabel").Text = _data.FishName;
        
        GD.Print($"Spawned fish node for: {_data.FishName}");
    }

    public override void _Ready()
    {
        ZIndex = 1;
        Modulate = new Color(1, 1, 1, 0.6f);
        // Create an Area2D for fishing interactions.
        var catchArea = new Area2D { Name = "CatchArea" };
        AddChild(catchArea);

        // Create a collision box for the Area2D.
        var capsuleShape = new CapsuleShape2D
        {
            Height = 44,
            Radius = 15
        };

        var collisionShape = new CollisionShape2D
        {
            Shape = capsuleShape,
            Position = new Vector2(2, 0),
            Rotation = Mathf.Pi / 2
        };

        catchArea.AddChild(collisionShape);
    }
}