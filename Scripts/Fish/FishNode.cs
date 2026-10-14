// FishNode.cs
using Godot;

public partial class FishNode : Sprite2D
{
    private FishData _data;

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

    private void _Ready()
    {
        // Create a collision box for the fish node
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

        AddChild(collisionShape);
    }
}