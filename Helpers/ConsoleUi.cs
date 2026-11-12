using MoneyTracker.Models;
using MoneyTracker.Services;

namespace MoneyTracker.Helpers;

// Draws the main dashboard: header, balance, menu choices, preview, and chart.
public static class ConsoleUi
{
    private const int InnerWidth = 74;
    private const int ActivityRows = 4;

    public static void DrawMainScreen(MoneyManager manager)
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            // Some hosts cannot clear the screen. The dashboard still prints.
        }

        decimal balance = manager.GetBalance();
        List<MoneyItem> all = manager.GetItems(null);

        WriteTopRule();
        WriteTitleRow();
        WriteBottomRule();

        Console.WriteLine();
        WriteBalanceCard(balance, all.Count);
        Console.WriteLine();
        WriteQuickOperations();
        Console.WriteLine();
        WriteLatestActivity(all);
        Console.WriteLine();
        WriteMonthlySpending(all);
        Console.WriteLine();
        WriteFooterHint();
    }

    public static void WriteSubheader(string title)
    {
        Console.WriteLine();
        ColoredText.WriteLine($"  ╭─ {title.ToUpperInvariant()} {new string('─', Math.Max(0, InnerWidth - title.Length - 6))}╮", ConsoleColor.Cyan);
        Console.WriteLine();
    }

    public static void WriteChoicePanel(string stepTitle, IReadOnlyList<(int number, string label, string hint, ConsoleColor accent)> options, string enterHint)
    {
        string rule = new string('─', InnerWidth - stepTitle.Length - 3);
        ColoredText.WriteLine($"  ┌─ {stepTitle} {rule}┐", ConsoleColor.DarkCyan);

        foreach ((int number, string label, string hint, ConsoleColor accent) in options)
        {
            Console.Write("  │  ");
            WriteChoiceBadge(number, accent);
            Console.Write(" ");
            ColoredText.Write(label, ConsoleColor.White);
            if (hint.Length > 0)
            {
                Console.Write("  ");
                ColoredText.Write(hint, ConsoleColor.DarkGray);
            }

            int used = 6 + 4 + label.Length + (hint.Length > 0 ? 2 + hint.Length : 0);
            int pad = InnerWidth - used + 2;
            if (pad < 0)
                pad = 0;
            Console.WriteLine(new string(' ', pad) + "│");
        }

        ColoredText.WriteLine($"  └{new string('─', InnerWidth)}┘", ConsoleColor.DarkCyan);
        ColoredText.WriteLine($"  {enterHint}", ConsoleColor.DarkGray);
    }

    public static void WriteHistoryResults(string summaryLine, IEnumerable<MoneyItem> items)
    {
        ColoredText.WriteLine($"  ┌─ RESULTS {new string('─', InnerWidth - 11)}┐", ConsoleColor.DarkCyan);
        Console.Write("  │  ");
        ColoredText.Write(summaryLine, ConsoleColor.Green);
        int summaryPad = InnerWidth - summaryLine.Length;
        if (summaryPad < 0)
            summaryPad = 0;
        Console.WriteLine(new string(' ', summaryPad) + "│");
        ColoredText.WriteLine($"  └{new string('─', InnerWidth)}┘", ConsoleColor.DarkCyan);
        Console.WriteLine();

        ColoredText.WriteLine($"  {"#",-4}{"Type",-10}{"Month",-12}{"Title",-22}{"Amount",12}", ConsoleColor.DarkGray);
        ColoredText.WriteLine($"  {new string('─', InnerWidth - 2)}", ConsoleColor.DarkGray);

        foreach (MoneyItem item in items)
        {
            string icon = IconFor(item);
            string title = TrimTo($"{icon} {item.Title}", 20);
            ConsoleColor amountColor = item.Kind == MoneyKind.Income ? ConsoleColor.Green : ConsoleColor.Red;

            Console.Write("  ");
            ColoredText.Write(item.Id.ToString().PadRight(4), ConsoleColor.Cyan);
            ColoredText.Write(item.Kind.ToString().PadRight(10), KindColor(item.Kind));
            Console.Write(item.MonthName.PadRight(12));
            Console.Write(title.PadRight(22));
            ColoredText.WriteLine(FormatItemAmount(item).PadLeft(12), amountColor);
        }
    }

    public static void WriteNotice(string message, ConsoleColor color)
    {
        ColoredText.WriteLine($"  ┃ {message}", color);
    }

    public static void WriteInlineError(string message)
    {
        WriteNotice(message, ConsoleColor.Red);
    }

    private static void WriteChoiceBadge(int number, ConsoleColor accent)
    {
        ConsoleColor previousBack = Console.BackgroundColor;
        ConsoleColor previousFore = Console.ForegroundColor;

        Console.BackgroundColor = accent;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" {number} ");
        Console.BackgroundColor = previousBack;
        Console.ForegroundColor = previousFore;
    }

    private static void WriteTopRule()
    {
        ColoredText.WriteLine($"  ╔{new string('═', InnerWidth)}╗", ConsoleColor.Blue);
    }

    private static void WriteBottomRule()
    {
        ColoredText.WriteLine($"  ╚{new string('═', InnerWidth)}╝", ConsoleColor.Blue);
    }

    private static void WriteTitleRow()
    {
        string left = "  TRACK MONEY";
        string divider = " │ ";
        string right = "PERSONAL ACCOUNTS";
        int padding = InnerWidth - left.Length - divider.Length - right.Length - 2;
        if (padding < 0)
            padding = 0;

        Console.Write("  ║");
        ColoredText.Write(left, ConsoleColor.White);
        ColoredText.Write(divider, ConsoleColor.DarkGray);
        ColoredText.Write(right, ConsoleColor.Cyan);
        Console.Write(new string(' ', padding));
        Console.WriteLine("║");
    }

    private static void WriteBalanceCard(decimal balance, int itemCount)
    {
        ColoredText.WriteLine($"  ┌─ ACCOUNT {new string('─', InnerWidth - 11)}┐", ConsoleColor.DarkCyan);

        Console.Write("  │  ");
        ColoredText.Write("AVAILABLE BALANCE", ConsoleColor.Gray);
        Console.WriteLine(new string(' ', InnerWidth - 20) + "│");

        string balanceText = FormatBalance(balance);
        ConsoleColor balanceColor = balance < 0 ? ConsoleColor.Red : ConsoleColor.Green;
        string status = balance < 0 ? "● Overdrawn" : "● In balance";
        ConsoleColor statusColor = balance < 0 ? ConsoleColor.Red : ConsoleColor.Green;

        Console.Write("  │  ");
        ColoredText.Write(balanceText, balanceColor);
        int gap = InnerWidth - 2 - balanceText.Length - status.Length;
        if (gap < 1)
            gap = 1;
        Console.Write(new string(' ', gap));
        ColoredText.Write(status, statusColor);
        Console.WriteLine(" │");

        string detail = $"Items saved in memory: {itemCount}";
        Console.Write("  │  ");
        ColoredText.Write(detail, ConsoleColor.DarkGray);
        int detailPad = InnerWidth - detail.Length;
        if (detailPad < 0)
            detailPad = 0;
        Console.WriteLine(new string(' ', detailPad) + "│");

        ColoredText.WriteLine($"  └{new string('─', InnerWidth)}┘", ConsoleColor.DarkCyan);
    }

    private static void WriteQuickOperations()
    {
        ColoredText.WriteLine($"  QUICK OPERATIONS", ConsoleColor.Yellow);
        WriteOperationLine('1', ConsoleColor.DarkGray, "View history", "All items, expenses only, or incomes only");
        WriteOperationLine('2', ConsoleColor.DarkGreen, "Add transaction", "New income or new expense");
        WriteOperationLine('3', ConsoleColor.DarkYellow, "Edit or remove", "Change one row or delete it");
        WriteOperationLine('4', ConsoleColor.DarkMagenta, "Save and quit", "Save JSON, report, backup, then exit");
    }

    private static void WriteOperationLine(char number, ConsoleColor bandColor, string title, string hint)
    {
        ConsoleColor previousBack = Console.BackgroundColor;
        ConsoleColor previousFore = Console.ForegroundColor;

        Console.Write("  ");
        Console.BackgroundColor = bandColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" {number} ");
        Console.BackgroundColor = previousBack;
        Console.ForegroundColor = previousFore;

        Console.Write(" ");
        ColoredText.Write(title, ConsoleColor.White);
        Console.Write("  ");
        ColoredText.WriteLine(hint, ConsoleColor.DarkGray);
    }

    private static void WriteLatestActivity(IReadOnlyList<MoneyItem> all)
    {
        ColoredText.WriteLine("  LATEST ACTIVITY", ConsoleColor.Yellow);

        if (all.Count == 0)
        {
            ColoredText.WriteLine("  (No rows yet. Pick 2 to add your first income or expense.)", ConsoleColor.DarkGray);
            return;
        }

        List<MoneyItem> recent = all
            .OrderByDescending(item => item.Id)
            .Take(ActivityRows)
            .ToList();

        WriteActivityHeader();
        foreach (MoneyItem item in recent)
            WriteActivityRow(item);

        if (all.Count > ActivityRows)
        {
            int more = all.Count - ActivityRows;
            string word = more == 1 ? "row" : "rows";
            ColoredText.WriteLine($"  … and {more} more {word}. Press 1 to view all.", ConsoleColor.DarkGray);
        }
    }

    private static void WriteMonthlySpending(IReadOnlyList<MoneyItem> all)
    {
        ColoredText.WriteLine("  MONTHLY SPENDING (expenses)", ConsoleColor.Yellow);

        decimal[] perMonth = new decimal[13];
        foreach (MoneyItem item in all)
        {
            if (item.Kind != MoneyKind.Expense || item.Month < 1 || item.Month > 12)
                continue;

            perMonth[item.Month] += item.Amount;
        }

        decimal max = 0;
        for (int month = 1; month <= 12; month++)
        {
            if (perMonth[month] > max)
                max = perMonth[month];
        }

        if (max <= 0)
        {
            ColoredText.WriteLine("  (No expenses yet, so there is nothing to chart.)", ConsoleColor.DarkGray);
            return;
        }

        // One ASCII character per month so columns stay aligned in Windows Terminal.
        const int barHeight = 4;
        for (int level = barHeight; level >= 1; level--)
        {
            Console.Write("  ");
            for (int month = 1; month <= 12; month++)
            {
                int height = BarHeight(perMonth[month], max, barHeight);
                bool filled = height >= level;
                ConsoleColor color = filled ? ConsoleColor.Blue : ConsoleColor.DarkGray;
                ColoredText.Write(filled ? "#" : ".", color);
            }

            Console.WriteLine();
        }

        Console.Write("  ");
        for (int month = 1; month <= 12; month++)
            ColoredText.Write(MonthLabel(month), ConsoleColor.DarkGray);

        Console.WriteLine();
        ColoredText.WriteLine("  Each letter is a month (J F M A …). More # signs mean more expenses.", ConsoleColor.DarkGray);
    }

    private static int BarHeight(decimal value, decimal max, int maxBars)
    {
        if (value <= 0 || max <= 0)
            return 0;

        double ratio = (double)(value / max);
        int bars = (int)Math.Round(ratio * maxBars, MidpointRounding.AwayFromZero);
        if (bars < 1 && value > 0)
            bars = 1;

        return Math.Min(bars, maxBars);
    }

    private static string MonthLabel(int month)
    {
        return month switch
        {
            1 => "J",
            2 => "F",
            3 => "M",
            4 => "A",
            5 => "M",
            6 => "J",
            7 => "J",
            8 => "A",
            9 => "S",
            10 => "O",
            11 => "N",
            12 => "D",
            _ => "?"
        };
    }

    private static void WriteActivityHeader()
    {
        Console.Write("  ");
        ColoredText.Write("Category".PadRight(22), ConsoleColor.DarkGray);
        ColoredText.Write("Type".PadRight(10), ConsoleColor.DarkGray);
        ColoredText.WriteLine("Amount".PadLeft(12), ConsoleColor.DarkGray);
    }

    private static void WriteActivityRow(MoneyItem item)
    {
        string icon = IconFor(item);
        string category = TrimTo($"{icon} {item.Title}", 20);
        string amount = FormatItemAmount(item);
        ConsoleColor amountColor = item.Kind == MoneyKind.Income ? ConsoleColor.Green : ConsoleColor.Red;

        Console.Write("  ");
        Console.Write(category.PadRight(22));
        ColoredText.Write(item.Kind.ToString().PadRight(10), KindColor(item.Kind));
        ColoredText.WriteLine(amount.PadLeft(12), amountColor);
    }

    private static void WriteFooterHint()
    {
        ColoredText.Write("  > Select operation ", ConsoleColor.Cyan);
        ColoredText.WriteLine("(1–4, or type exit / quit / q)", ConsoleColor.DarkGray);
    }

    private static string FormatBalance(decimal balance)
    {
        if (balance < 0)
            return $"(-){Math.Abs(balance):0.00} kr";

        return $"{balance:0.00} kr";
    }

    private static string FormatItemAmount(MoneyItem item)
    {
        return $"{item.Amount:0.00} kr";
    }

    private static string IconFor(MoneyItem item)
    {
        return item.Kind == MoneyKind.Income ? "+" : "-";
    }

    private static string TrimTo(string text, int max)
    {
        if (text.Length <= max)
            return text;

        if (max <= 1)
            return text[..max];

        return text[..(max - 1)] + "…";
    }

    private static ConsoleColor KindColor(MoneyKind kind)
    {
        return kind == MoneyKind.Income ? ConsoleColor.Green : ConsoleColor.Red;
    }
}