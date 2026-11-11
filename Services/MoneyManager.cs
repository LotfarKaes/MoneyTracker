using MoneyTracker.Models;

namespace MoneyTracker.Services;

// Keeps the money items in memory while the program is running.
public class MoneyManager
{
    private readonly List<MoneyItem> items = new List<MoneyItem>();
    private int nextId = 1;

    public int NextId => nextId;

    public bool AddItem(MoneyItem item)
    {
        if (!CanStore(item) || FindById(item.Id) != null)
            return false;

        item.Title = item.Title.Trim();
        items.Add(item);

        if (item.Id >= nextId)
            nextId = item.Id + 1;

        return true;
    }

    public int LoadItems(IEnumerable<MoneyItem> loadedItems, int nextFreeId)
    {
        items.Clear();
        int skipped = 0;

        foreach (MoneyItem item in loadedItems)
        {
            if (!CanStore(item) || FindById(item.Id) != null)
            {
                skipped++;
                continue;
            }

            item.Title = item.Title.Trim();
            items.Add(item);
        }

        int highest = 0;
        foreach (MoneyItem item in items)
        {
            if (item.Id > highest)
                highest = item.Id;
        }

        nextId = nextFreeId > highest ? nextFreeId : highest + 1;
        if (nextId < 1)
            nextId = 1;

        return skipped;
    }

    public List<MoneyItem> GetItems(MoneyKind? kindFilter)
    {
        if (kindFilter == null)
            return items.ToList();

        return items.Where(item => item.Kind == kindFilter).ToList();
    }

    public void SortItems(string sortBy, bool ascending)
    {
        string key = sortBy.Trim().ToLower();
        IOrderedEnumerable<MoneyItem> sorted;

        if (key == "amount")
        {
            sorted = ascending
                ? items.OrderBy(item => item.Amount)
                : items.OrderByDescending(item => item.Amount);
        }
        else if (key == "title")
        {
            sorted = ascending
                ? items.OrderBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
                : items.OrderByDescending(item => item.Title, StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            sorted = ascending
                ? items.OrderBy(item => item.Month)
                : items.OrderByDescending(item => item.Month);
        }

        List<MoneyItem> ordered = sorted.ThenBy(item => item.Id).ToList();
        items.Clear();
        items.AddRange(ordered);
    }

    public bool UpdateItem(int id, string title, decimal amount, int month, MoneyKind kind)
    {
        MoneyItem? item = FindById(id);
        if (item == null || !HasRequiredFields(title, amount, month))
            return false;

        item.Title = title.Trim();
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
        decimal income = items
            .Where(item => item.Kind == MoneyKind.Income)
            .Sum(item => item.Amount);

        decimal expenses = items
            .Where(item => item.Kind == MoneyKind.Expense)
            .Sum(item => item.Amount);

        return income - expenses;
    }

    public MoneyItem? FindById(int id)
    {
        return items.FirstOrDefault(item => item.Id == id);
    }

    private static bool CanStore(MoneyItem item)
    {
        return item != null
            && item.Id > 0
            && HasRequiredFields(item.Title, item.Amount, item.Month);
    }

    // Same rule as the questions on screen: a row needs a title, an amount above 0, and a month from 1 to 12.
    private static bool HasRequiredFields(string title, decimal amount, int month)
    {
        return !string.IsNullOrWhiteSpace(title)
            && amount > 0
            && month >= 1
            && month <= 12;
    }
}