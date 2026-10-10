using Godot;

public partial class SpawnService : Node
{
    [Export] private FishData[] fishPool;
    //[Export] private TreasureData[] treasurePool;

    public Resource SpawnRandomEntity(SpawnCategory category)
    {
        // Handles probability math and returns the appropriate resource
    }
}