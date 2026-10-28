using Godot;
using Incremental.Game.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Incremental.Game.Scripts.Services;

public interface IScoreWallet
{
    double Score { get; }
    bool TrySpendScore(double amount);
}

public partial class EconomyService : Node, IScoreWallet
{
    private FishingService _fishingService;
    private const string ResourcesPath = "res://Game/Resources/Game";

    private ResourceInventory _inventory = new();
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

            _inventory.Register(resourceName);
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
        return _inventory.Amounts;
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
        return _inventory.ResourceTypeCount;
    }

    public int GetResourceAmount(string resourceName)
    {
        return _inventory.GetAmount(resourceName);
    }

    public void AddResource(string resourceName, int amount)
    {
        if (amount <= 0)
        {
            GD.PrintErr($"Attempted to add a non-positive amount of resource '{resourceName}': {amount}");
            return;
        }

        _inventory.Add(resourceName, amount);
    }

    public bool TrySpendScore(double amount)
    {
        if (!double.IsFinite(amount) || amount <= 0d)
        {
            GD.PrintErr($"Attempted to spend an invalid amount of score: {amount}");
            return false;
        }

        if (Score < amount)
            return false;

        Score -= amount;
        ScoreChanged?.Invoke(Score);
        return true;
    }

    public void AddScore(double amount)
    {
        if (!double.IsFinite(amount) || amount <= 0d)
        {
            GD.PrintErr($"Attempted to add an invalid amount of score: {amount}");
            return;
        }

        Score += amount;
        ScoreChanged?.Invoke(Score);
    }

    public bool SpendResource(string resourceName, int amount)
    {
        if (amount <= 0)
        {
            GD.PrintErr($"Attempted to spend a non-positive amount of resource '{resourceName}': {amount}");
            return false;
        }
        
        int currentAmount = _inventory.GetAmount(resourceName);
        if (!_inventory.Amounts.ContainsKey(resourceName))
        {
            GD.PrintErr($"Not enough of resource '{resourceName}' to spend. Available: 0, Attempted to spend: {amount}");
            return false;
        }
        
        if (currentAmount < amount)
        {
            GD.PrintErr($"Not enough of resource '{resourceName}' to spend. Available: {currentAmount}, Attempted to spend: {amount}");
            return false;
        }
        
        return _inventory.TrySpend(resourceName, amount);
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
            AddScore(valuable.BaseValue);
    }
}