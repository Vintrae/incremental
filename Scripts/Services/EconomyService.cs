using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class EconomyService : Node
{
    private FishingService _fishingService;
    private const string ResourcesPath = "res://Resources/Game";

    private Dictionary<string, int> ResourceAmounts { get; } = new Dictionary<string, int>();

    public double Score { get; private set; }
    public event Action<double> ScoreChanged;

    public override void _Ready()
    {
        _fishingService = GetNode<FishingService>("../FishingService");
        _fishingService.EntityCaught += OnEntityCaught;
        LoadResources(ResourcesPath);
    }

    private void LoadResources(string path)
    {
        using var directory = DirAccess.Open(path);

        if (directory == null)
        {
            GD.PrintErr($"Failed to open directory: {path}");
            return;
        }

        foreach (string file in directory.GetFiles())
        {
            if (!file.EndsWith(".tres")) continue;

            string resourceName = Path.GetFileNameWithoutExtension(file);
            ResourceAmounts.TryAdd(resourceName, 0);
        }

        foreach (string subdirectory in directory.GetDirectories())
        {
            LoadResources($"{path}/{subdirectory}");
        }
    }

    public int GetResourceTypeCount()
    {
        return ResourceAmounts.Count;
    }

    public int GetResourceAmount(string resourceName)
    {
        return ResourceAmounts.TryGetValue(resourceName, out int amount) ? amount : 0;
    }

    public void AddResource(string resourceName, int amount)
    {
        if (amount <= 0)
        {
            GD.PrintErr($"Attempted to add a non-positive amount of resource '{resourceName}': {amount}");
            return;
        }

        if (ResourceAmounts.ContainsKey(resourceName))
            ResourceAmounts[resourceName] += amount;
    }

    public bool SpendResource(string resourceName, int amount)
    {
        if (amount <= 0)
        {
            GD.PrintErr($"Attempted to spend a non-positive amount of resource '{resourceName}': {amount}");
            return false;
        }
        
        if (!ResourceAmounts.TryGetValue(resourceName, out int currentAmount))
        {
            GD.PrintErr($"Not enough of resource '{resourceName}' to spend. Available: {ResourceAmounts[resourceName]}, Attempted to spend: {amount}");
            return false;
        }
        
        if (currentAmount < amount)
        {
            GD.PrintErr($"Not enough of resource '{resourceName}' to spend. Available: {currentAmount}, Attempted to spend: {amount}");
            return false;
        }
        
        ResourceAmounts[resourceName] = currentAmount - amount;
        return true;
    }

    private void OnEntityCaught(Node entity)
    {
        if (entity is IValuable valuable)
        {
            Score += valuable.BaseValue;
            ScoreChanged?.Invoke(Score);
        }
    }
}