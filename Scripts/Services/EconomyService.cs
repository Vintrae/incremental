using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class EconomyService : Node
{
    private FishingService _fishingService;
    private const string ResourcesPath = "res://Resources/Game";

    private Dictionary<string, int> ResourceAmounts { get; } = new Dictionary<string, int>();
    private Dictionary<string, Resource> ResourceDefinitions { get; } = new Dictionary<string, Resource>();
    private Dictionary<string, string> ResourceNamesByPath { get; } = new Dictionary<string, string>();

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
            var resourcePath = $"{path}/{file}";
            var resourceDefinition = GD.Load<Resource>(resourcePath);
            if (resourceDefinition == null)
            {
                GD.PrintErr($"Failed to load inventory resource definition: {resourcePath}");
                continue;
            }

            ResourceAmounts.TryAdd(resourceName, 0);
            ResourceDefinitions.TryAdd(resourceName, resourceDefinition);
            ResourceNamesByPath.TryAdd(resourceDefinition.ResourcePath, resourceName);
        }

        foreach (string subdirectory in directory.GetDirectories())
        {
            LoadResources($"{path}/{subdirectory}");
        }
    }

    public IReadOnlyDictionary<string, int> GetResourceInventory()
    {
        return ResourceAmounts;
    }

    public Texture2D GetResourceIcon(string resourceName)
    {
        return ResourceDefinitions.TryGetValue(resourceName, out Resource definition) &&
            definition is IInventoryResourceDefinition inventoryResource
                ? inventoryResource.InventoryIcon
                : null;
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
        if (entity is IInventoryResourceDrop resourceDrop && resourceDrop.InventoryResource != null)
        {
            if (ResourceNamesByPath.TryGetValue(resourceDrop.InventoryResource.ResourcePath, out string resourceName))
                AddResource(resourceName, 1);
            else
                GD.PrintErr($"Caught resource is not registered in the inventory: {resourceDrop.InventoryResource.ResourcePath}");
        }

        if (entity is IValuable valuable)
        {
            Score += valuable.BaseValue;
            ScoreChanged?.Invoke(Score);
        }
    }
}