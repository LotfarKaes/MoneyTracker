using MoneyTracker.Models;
using MoneyTracker.Services;

namespace MoneyTracker.Helpers;

// The screen: menu, show, add, edit, and remove.
public class Menu
{
    public void Run(MoneyManager manager)
    {
        bool running = true;

        while (running)
        {
            PrintMenu(manager.GetBalance());
            int choice = InputHelper.ReadMenuChoice(">> ", 1, 4);

            if (InputHelper.ExitRequested || choice == 4)
            {
                SayGoodbye(manager);
                break;
            }

            switch (choice)
            {
                case 1:
                    ShowChosenItems(manager);
                    break;
                case 2:
                    AddNewItem(manager);
                    break;
                case 3:
                    EditOrRemove(manager);
                    break;
                default:
                    ColoredText.WriteLine(">> Please type 1, 2, 3, 4, or exit.", ConsoleColor.Red);
                    break;
            }

            if (InputHelper.ExitRequested)
            {
                SayGoodbye(manager);
                running = false;
            }
        }
    }

    private static void PrintMenu(decimal balance)
    {
        Console.WriteLine();
        ColoredText.WriteLine(">> Welcome to TrackMoney", ConsoleColor.Cyan);

        ConsoleColor balanceColor = balance < 0 ? ConsoleColor.Red : ConsoleColor.Green;
        ColoredText.WriteLine($">> You have currently {FormatBalance(balance)} on your account.", balanceColor);

        ColoredText.WriteLine(">> Pick an option:", ConsoleColor.Cyan);
        ColoredText.WriteLine(">> (1) Show items (All/Expense(s)/Income(s))", ConsoleColor.Cyan);
        ColoredText.WriteLine(">> (2) Add New Expense/Income", ConsoleColor.Cyan);
        ColoredText.WriteLine(">> (3) Edit Item (edit, remove)", ConsoleColor.Cyan);
        ColoredText.WriteLine(">> (4) Save and Quit", ConsoleColor.Cyan);
    }

    private static string FormatBalance(decimal balance)
    {
        if (balance < 0)
            return $"(-){Math.Abs(balance):0.00} kr";

        return $"{balance:0.00} kr";
    }

    private static void ShowAllItems(MoneyManager manager)
    {
        List<MoneyItem> items = manager.GetItems(null);

        if (items.Count == 0)
        {
            ColoredText.WriteLine(">> There are no items yet.", ConsoleColor.Yellow);
            return;
        }

        ColoredText.WriteLine($">> Showing all {items.Count} items:", ConsoleColor.Green);

        foreach (MoneyItem item in items)
            ColoredText.WriteLine(">> " + item, KindColor(item.Kind));
    }

    private static void ShowChosenItems(MoneyManager manager)
    {
        if (manager.GetItems(null).Count == 0)
        {
            ColoredText.WriteLine(">> There are no items yet.", ConsoleColor.Yellow);
            return;
        }

        int which = Ask(new string[]
        {
            ">> Show:",
            ">> (1) All",
            ">> (2) Expenses",
            ">> (3) Incomes",
            ">> Press Enter for all."
        }, 3, 1);

        if (InputHelper.ExitRequested)
            return;

        int sortChoice = Ask(new string[]
        {
            ">> Sort by:",
            ">> (1) Month",
            ">> (2) Amount",
            ">> (3) Title",
            ">> Press Enter for month."
        }, 3, 1);

        if (InputHelper.ExitRequested)
            return;

        int orderChoice = Ask(new string[]
        {
            ">> Order:",
            ">> (1) Ascending",
            ">> (2) Descending",
            ">> Press Enter for ascending."
        }, 2, 1);

        if (InputHelper.ExitRequested)
            return;

        string sortBy = "month";
        if (sortChoice == 2)
            sortBy = "amount";
        else if (sortChoice == 3)
            sortBy = "title";

        bool ascending = orderChoice == 1;
        manager.SortItems(sortBy, ascending);

        MoneyKind? filter = null;
        if (which == 2)
            filter = MoneyKind.Expense;
        else if (which == 3)
            filter = MoneyKind.Income;

        List<MoneyItem> items = manager.GetItems(filter);
        if (items.Count == 0)
        {
            string missing = filter == MoneyKind.Income ? "incomes" : "expenses";
            ColoredText.WriteLine($">> There are no {missing} yet.", ConsoleColor.Yellow);
            return;
        }

        ColoredText.WriteLine(
            $">> Showing {items.Count} {WordForCount(items.Count, filter)}, sorted by {sortBy}, {OrderWords(sortBy, ascending)}:",
            ConsoleColor.Green);

        foreach (MoneyItem item in items)
            ColoredText.WriteLine(">> " + item, KindColor(item.Kind));
    }

    private static int Ask(string[] lines, int max, int defaultChoice)
    {
        while (!InputHelper.ExitRequested)
        {
            foreach (string line in lines)
                ColoredText.WriteLine(line, ConsoleColor.Cyan);

            int choice = InputHelper.ReadMenuChoice(">> ", 1, max, true);
            if (InputHelper.ExitRequested)
                return 0;

            if (choice == -1)
                return defaultChoice;

            if (choice != 0)
                return choice;

            ColoredText.WriteLine($">> Please type 1 to {max}, or press Enter.", ConsoleColor.Red);
        }

        return 0;
    }

    private static string WordForCount(int count, MoneyKind? filter)
    {
        if (filter == MoneyKind.Income)
            return count == 1 ? "income" : "incomes";

        if (filter == MoneyKind.Expense)
            return count == 1 ? "expense" : "expenses";

        return count == 1 ? "item" : "items";
    }

    private static string OrderWords(string sortBy, bool ascending)
    {
        if (sortBy == "amount")
            return ascending ? "small to large" : "large to small";

        if (sortBy == "title")
            return ascending ? "A to Z" : "Z to A";

        return ascending ? "January to December" : "December to January";
    }

    private static void AddNewItem(MoneyManager manager)
    {
        ColoredText.WriteLine(">> (1) Income", ConsoleColor.Green);
        ColoredText.WriteLine(">> (2) Expense", ConsoleColor.Red);

        MoneyKind? kind = InputHelper.ReadKind(">> ");
        string? title = kind == null ? null : InputHelper.ReadText(">> Title: ");
        decimal? amount = title == null ? null : InputHelper.ReadAmount(">> Amount: ");
        int? month = amount == null ? null : InputHelper.ReadMonth(">> Month (1-12 or name): ");

        if (kind == null || title == null || amount == null || month == null)
            return;

        MoneyItem item = new MoneyItem(manager.NextId, title, amount.Value, month.Value, kind.Value);
        if (!manager.AddItem(item))
        {
            ColoredText.WriteLine(">> That item was not added. It needs a title, an amount above 0, and a month from 1 to 12.", ConsoleColor.Red);
            return;
        }

        ColoredText.WriteLine(">> Added " + item, ConsoleColor.Green);
    }

    private static void EditOrRemove(MoneyManager manager)
    {
        if (manager.GetItems(null).Count == 0)
        {
            ColoredText.WriteLine(">> There are no items yet.", ConsoleColor.Yellow);
            return;
        }

        ShowAllItems(manager);

        MoneyItem? item = null;
        while (item == null && !InputHelper.ExitRequested)
        {
            int id = InputHelper.ReadItemNumber(">> Item number: ");
            if (InputHelper.ExitRequested)
                return;

            item = manager.FindById(id);
            if (item == null)
                ColoredText.WriteLine($">> No item with number {id}.", ConsoleColor.Red);
        }

        if (item == null)
            return;

        ColoredText.WriteLine(">> (1) Edit", ConsoleColor.Cyan);
        ColoredText.WriteLine(">> (2) Remove", ConsoleColor.Cyan);
        int action = InputHelper.ReadMenuChoice(">> ", 1, 2);

        if (InputHelper.ExitRequested)
            return;

        if (action == 1)
            EditItem(manager, item);
        else if (action == 2)
            ConfirmRemove(manager, item);
        else
            ColoredText.WriteLine(">> Please type 1 to edit or 2 to remove.", ConsoleColor.Red);
    }

    private static void EditItem(MoneyManager manager, MoneyItem item)
    {
        ColoredText.WriteLine(">> Editing " + item, KindColor(item.Kind));
        ColoredText.WriteLine(">> Press Enter to keep the old value.", ConsoleColor.Yellow);

        string? title = InputHelper.ReadText($">> Title [{item.Title}]: ", item.Title);
        decimal? amount = title == null
            ? null
            : InputHelper.ReadAmount($">> Amount [{item.Amount:0.00}]: ", item.Amount);
        int? month = amount == null
            ? null
            : InputHelper.ReadMonth($">> Month [{item.MonthName}]: ", item.Month);
        MoneyKind? kind = month == null
            ? null
            : InputHelper.ReadKind($">> Kind [{item.Kind}] (1 income, 2 expense): ", item.Kind);

        if (title == null || amount == null || month == null || kind == null)
            return;

        bool updated = manager.UpdateItem(item.Id, title, amount.Value, month.Value, kind.Value);
        if (!updated)
        {
            ColoredText.WriteLine(">> Could not update that item. It needs a title, an amount above 0, and a month from 1 to 12.", ConsoleColor.Red);
            return;
        }

        ColoredText.WriteLine(">> Updated " + item, ConsoleColor.Green);
    }

    private static void ConfirmRemove(MoneyManager manager, MoneyItem item)
    {
        bool? yes = InputHelper.ReadYesNo($">> Remove {item.Title}? (yes/no): ");
        if (yes != true)
        {
            if (!InputHelper.ExitRequested)
                ColoredText.WriteLine(">> Kept the item.", ConsoleColor.Yellow);
            return;
        }

        if (manager.RemoveItem(item.Id))
            ColoredText.WriteLine($">> Removed {item.Title}.", ConsoleColor.Green);
        else
            ColoredText.WriteLine(">> Could not remove that item.", ConsoleColor.Red);
    }

    private static void SayGoodbye(MoneyManager manager)
    {
        FileHandler files = new FileHandler();
        List<MoneyItem> items = manager.GetItems(null);
        files.Save(items, manager.NextId);
        files.WriteCleanReport(items);
        ColoredText.WriteLine(">> Goodbye.", ConsoleColor.Cyan);
    }

    private static ConsoleColor KindColor(MoneyKind kind)
    {
        return kind == MoneyKind.Income ? ConsoleColor.Green : ConsoleColor.Red;
    }
}