using System.Collections.Generic;

public sealed class ResourceInventory
{
    private readonly Dictionary<string, int> _amounts = new();

    public IReadOnlyDictionary<string, int> Amounts => _amounts;
    public int ResourceTypeCount => _amounts.Count;

    public void Register(string resourceName)
    {
        _amounts.TryAdd(resourceName, 0);
    }

    public int GetAmount(string resourceName)
    {
        return _amounts.TryGetValue(resourceName, out int amount) ? amount : 0;
    }

    public bool Add(string resourceName, int amount)
    {
        if (amount <= 0 || !_amounts.TryGetValue(resourceName, out int currentAmount))
            return false;

        _amounts[resourceName] = currentAmount + amount;
        return true;
    }

    public bool TrySpend(string resourceName, int amount)
    {
        if (amount <= 0 || !_amounts.TryGetValue(resourceName, out int currentAmount) ||
            currentAmount < amount)
            return false;

        _amounts[resourceName] = currentAmount - amount;
        return true;
    }
}
