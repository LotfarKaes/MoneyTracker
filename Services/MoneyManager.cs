using InPro1MoneyTracker.Models;

namespace InPro1MoneyTracker.Services;

// Keeps the money items in memory while the program is running.
// Edit, remove, sort, and the balance are filled in later steps.
public class MoneyManager
{
    private readonly List<MoneyItem> items = new List<MoneyItem>();

    public void AddItem(MoneyItem item)
    {
        items.Add(item);
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

    public bool UpdateItem(int id, MoneyItem updated)
    {
        // Step 6: change the item with this id.
        return false;
    }

    public bool RemoveItem(int id)
    {
        // Step 6: remove the item with this id.
        return false;
    }

    public decimal GetBalance()
    {
        // Step 6: all income minus all expenses.
        return 0m;
    }

    public MoneyItem? FindById(int id)
    {
        // Step 6: find one item, or return nothing when the id is missing.
        return null;
    }
}