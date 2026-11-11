using MoneyTracker.Models;

namespace MoneyTracker.Services;

// Keeps the money items in memory while the program is running.
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

    public int LoadItems(IEnumerable<MoneyItem> loadedItems, int nextFreeId)
    {
        items.Clear();
        int skipped = 0;

        foreach (MoneyItem item in loadedItems)
        {
            if (item == null || item.Id <= 0 || FindById(item.Id) != null)
            {
                skipped++;
                continue;
            }

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
        List<MoneyItem> result = new List<MoneyItem>();

        foreach (MoneyItem item in items)
        {
            if (kindFilter == null || item.Kind == kindFilter)
                result.Add(item);
        }

        return result;
    }

    public void SortItems(string sortBy, bool ascending)
    {
        items.Sort((first, second) =>
        {
            int compared = Compare(first, second, sortBy);
            if (compared == 0)
                return first.Id.CompareTo(second.Id);

            return ascending ? compared : -compared;
        });
    }

    private static int Compare(MoneyItem first, MoneyItem second, string sortBy)
    {
        string key = sortBy.Trim().ToLower();

        if (key == "amount")
            return first.Amount.CompareTo(second.Amount);

        if (key == "title")
            return string.Compare(first.Title, second.Title, StringComparison.OrdinalIgnoreCase);

        return first.Month.CompareTo(second.Month);
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