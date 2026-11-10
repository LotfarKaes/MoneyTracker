using InPro1MoneyTracker.Helpers;
using InPro1MoneyTracker.Models;
using InPro1MoneyTracker.Services;

// Step 4 stores several MoneyItem objects in one list.
// The menu comes in step 5.

Console.OutputEncoding = System.Text.Encoding.UTF8;

ColoredText.WriteLine(">> Welcome to TrackMoney", ConsoleColor.Cyan);

MoneyManager manager = new MoneyManager();
manager.AddItem(new MoneyItem(1, "Salary", 25000m, 1, MoneyKind.Income));
manager.AddItem(new MoneyItem(2, "Rent", 8000m, 1, MoneyKind.Expense));
manager.AddItem(new MoneyItem(3, "Food", 450m, 3, MoneyKind.Expense));

List<MoneyItem> items = manager.GetItems(null);

ColoredText.WriteLine($">> {items.Count} items are in the list:", ConsoleColor.Green);

foreach (MoneyItem item in items)
{
    ConsoleColor rowColor = item.Kind == MoneyKind.Income
        ? ConsoleColor.Green
        : ConsoleColor.Red;

    ColoredText.WriteLine(">> " + item, rowColor);
}

ColoredText.WriteLine(">> The menu comes in the next step.", ConsoleColor.Yellow);