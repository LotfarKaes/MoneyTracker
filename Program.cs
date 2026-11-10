using InPro1MoneyTracker.Helpers;
using InPro1MoneyTracker.Models;
using InPro1MoneyTracker.Services;

// Step 5 shows the menu again and again until the user quits.
// Add, edit, and remove are filled in on the next step.

Console.OutputEncoding = System.Text.Encoding.UTF8;

MoneyManager manager = new MoneyManager();
AddSampleItems(manager);

bool running = true;

while (running)
{
    PrintMenu(manager.GetBalance());
    int choice = InputHelper.ReadMenuChoice(">> ", 1, 4);

    switch (choice)
    {
        case 1:
            ShowAllItems(manager);
            break;
        case 2:
            ColoredText.WriteLine(">> Adding a new item comes in the next step.", ConsoleColor.Yellow);
            break;
        case 3:
            ColoredText.WriteLine(">> Edit and remove come in the next step.", ConsoleColor.Yellow);
            break;
        case 4:
            ColoredText.WriteLine(">> Saving to a file comes in a later step.", ConsoleColor.Yellow);
            ColoredText.WriteLine(">> Goodbye.", ConsoleColor.Cyan);
            running = false;
            break;
        default:
            ColoredText.WriteLine(">> Please type 1, 2, 3, 4, or exit.", ConsoleColor.Red);
            break;
    }
}

static void AddSampleItems(MoneyManager manager)
{
    manager.AddItem(new MoneyItem(1, "Salary", 25000m, 1, MoneyKind.Income));
    manager.AddItem(new MoneyItem(2, "Rent", 8000m, 1, MoneyKind.Expense));
    manager.AddItem(new MoneyItem(3, "Food", 450m, 3, MoneyKind.Expense));
}

static void PrintMenu(decimal balance)
{
    Console.WriteLine();
    ColoredText.WriteLine(">> Welcome to TrackMoney", ConsoleColor.Cyan);

    ConsoleColor balanceColor = balance < 0 ? ConsoleColor.Red : ConsoleColor.Green;
    ColoredText.WriteLine($">> You have currently {FormatBalance(balance)} on your account.", balanceColor);

    Console.WriteLine(">> Pick an option:");
    Console.WriteLine(">> (1) Show items (All/Expense(s)/Income(s))");
    Console.WriteLine(">> (2) Add New Expense/Income");
    Console.WriteLine(">> (3) Edit Item (edit, remove)");
    Console.WriteLine(">> (4) Save and Quit");
}

static string FormatBalance(decimal balance)
{
    if (balance < 0)
        return $"(-){Math.Abs(balance):0.00} kr";

    return $"{balance:0.00} kr";
}

static void ShowAllItems(MoneyManager manager)
{
    List<MoneyItem> items = manager.GetItems(null);

    if (items.Count == 0)
    {
        ColoredText.WriteLine(">> There are no items yet.", ConsoleColor.Yellow);
        return;
    }

    ColoredText.WriteLine($">> Showing all {items.Count} items:", ConsoleColor.Green);

    foreach (MoneyItem item in items)
    {
        ConsoleColor rowColor = item.Kind == MoneyKind.Income
            ? ConsoleColor.Green
            : ConsoleColor.Red;

        ColoredText.WriteLine(">> " + item, rowColor);
    }
}