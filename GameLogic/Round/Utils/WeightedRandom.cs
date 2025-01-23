namespace GameLogic.Round.Utils;

public class WeightedRandom<T>
{
    private readonly List<(T Item, int CumulativeWeight)> _weightedItems = new();
    private readonly Random _random = new();
    private readonly int _totalWeight = 0;
    
    public WeightedRandom(IEnumerable<(T Item, int Weight)> items)
    {
        foreach (var itemChance in items)
        {
            _totalWeight += itemChance.Weight;
            _weightedItems.Add((itemChance.Item, _totalWeight));
        }
    }

    public T GetRandomItem()
    {
        if (_weightedItems.Count == 0)
        {
            throw new InvalidOperationException("No items have been added");
        }

        int randomValue = _random.Next(_totalWeight);
        foreach (var itemChance in _weightedItems)
        {
            if (randomValue < itemChance.CumulativeWeight)
            {
                return itemChance.Item;
            }
        }
        
        throw new InvalidOperationException("Failed to get random item");
    }
}