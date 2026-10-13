using Godot;

public partial class SpawnService : Node
{
    [Export] private FishData[] fishPool;
    [Export] private int maxFishOnScreen = 10;
    private Timer _spawnTimer;
    //[Export] private Resource[] treasurePool; // placeholder for other pools

    public Resource SpawnEntity(SpawnCategory category, string entityName)
    {
        switch (category)
        {
            case SpawnCategory.Fish:
                if (fishPool == null || fishPool.Length == 0) return null;
                FishData targetFish = System.Array.Find(fishPool, fish => fish.FishName.ToLower() == entityName.ToLower());
                if (!targetFish.IsUnlocked) return null;
                return null;


            // Add other categories here
            default:
                return null;
        }
    }
    
    public Resource SpawnRandomEntity(SpawnCategory category)
    {
        switch (category)
        {
            case SpawnCategory.Fish:
                if (fishPool == null || fishPool.Length == 0) return null;
                //var rng = new RandomUtil();
                
                // 1. Pick a random fish from the pool based on their weights
                FishData targetFish = WeightedRandom.PickByWeight(fishPool);

                // 2. Load the reusable FishNode scene template
                var fishSceneBlueprint = GD.Load<PackedScene>("res://Scenes/FishNode.tscn");
                if (fishSceneBlueprint == null) return null;

                // 3. Instantiate the scene into a live node
                var newFishNode = fishSceneBlueprint.Instantiate<FishNode>();

                // 4. Pass the FishData resource to it so it configures its sprite/name and add to group
                newFishNode.Initialize(targetFish);
                newFishNode.AddToGroup("fish");

                // 5. Set its position and add it to the active game world tree
                newFishNode.GlobalPosition = new Vector2(GD.RandRange(0, 720), GD.RandRange(0, 1280));

                // Adds it to the current active scene root
                GetTree().CurrentScene.AddChild(newFishNode);
                return targetFish;

            // Add other categories here
            default:
                return null;
        }
    }
    public override void _Ready()
    {
        // Get the timer child node (or create one programmatically)
        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
    }

    private void OnSpawnTimerTimeout()
    {
        // 1. Check current count using Godot's built-in group system
        int currentFishCount = GetTree().GetNodesInGroup("fish").Count;

        // 2. Only spawn if we are under the threshold
        if (currentFishCount < maxFishOnScreen)
        {
            SpawnRandomEntity(SpawnCategory.Fish);
        }
    }
}
