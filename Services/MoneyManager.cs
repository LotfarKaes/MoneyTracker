using InPro1MoneyTracker.Models;

namespace InPro1MoneyTracker.Services;

// Keeps the money items while the program is running.
// The list itself is added in step 4. The method bodies come in steps 6 and 7.
public class MoneyManager
{
    public void AddItem(MoneyItem item)
    {
        // Step 6: add one item to the list.
    }

    public List<MoneyItem> GetItems(MoneyKind? kindFilter)
    {
        // Step 6 shows every row. Step 7 uses kindFilter for income or expense only.
        return new List<MoneyItem>();
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