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
}