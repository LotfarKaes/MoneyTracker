using InPro1MoneyTracker.Models;

namespace InPro1MoneyTracker.Services;

// Keeps the money items in memory while the program is running.
// Sort, and showing only one kind, come in the next step.
public class MoneyManager
{
    private readonly List<MoneyItem> items = new List<MoneyItem>();
    private int nextId = 1;

    public int NextId => nextId;

    public void AddItem(MoneyItem item)
    {
        items.Add(item);

        if (item.Id >= nextId)
            nextId = item.Id + 1;
    }

    public List<MoneyItem> GetItems(MoneyKind? kindFilter)
    {
        // Step 7 uses kindFilter to keep only incomes or only expenses.
        return new List<MoneyItem>(items);
    }

    public void SortItems(string sortBy, bool ascending)
    {
        // Step 7: sort by month, amount, or title.
    }

    public bool UpdateItem(int id, string title, decimal amount, int month, MoneyKind kind)
    {
        MoneyItem? item = FindById(id);
        if (item == null)
            return false;

        item.Title = title;
        item.Amount = amount;
        item.Month = month;
        item.Kind = kind;
        return true;
    }

    public bool RemoveItem(int id)
    {
        MoneyItem? item = FindById(id);
        if (item == null)
            return false;

        items.Remove(item);
        return true;
    }

    public decimal GetBalance()
    {
        decimal balance = 0m;

        foreach (MoneyItem item in items)
        {
            if (item.Kind == MoneyKind.Income)
                balance += item.Amount;
            else
                balance -= item.Amount;
        }

        return balance;
    }

    public MoneyItem? FindById(int id)
    {
        foreach (MoneyItem item in items)
        {
            if (item.Id == id)
                return item;
        }

        return null;
    }
}