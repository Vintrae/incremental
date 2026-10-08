using Godot;
using System.Collections.Generic;



public partial class FishDirector : Node
{
    // Create list of fishes.
    private readonly List<Fish> _activeFishes = new List<Fish>();

    public Fish RegisterFish()
    {
        var registeredFish = new Fish();
        _activeFishes.Add(registeredFish);
        return registeredFish;
    }

    public List<Fish> UnegisterFish(Fish fish)
    {
        if (_activeFishes.Contains(fish))
        {
            _activeFishes.Remove(fish);
        }
        return _activeFishes;
    }

    public Vector2[] GetFishLocations()
    {
        var locations = new List<Vector2>();
        foreach (var fish in _activeFishes)
        {
            if (GodotObject.IsInstanceValid(fish))
            {
                locations.Add(fish.Position);
            }
        }
        return locations.ToArray();
    }

    public override void _Ready()
    {
    } 
}