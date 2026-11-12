using System.Globalization;
using MoneyTracker.Models;

namespace MoneyTracker.Helpers;

// Reads what the user types. Asks again when the answer does not fit.
public static class InputHelper
{
    public static bool ExitRequested { get; private set; }

    // Keeps the last screen visible until the user is ready for the dashboard again.
    public static void WaitReturnToMainMenu()
    {
        if (ExitRequested)
            return;

        Console.WriteLine();
        ColoredText.WriteLine("  Press Enter to return to the main menu…", ConsoleColor.DarkGray);
        string? line = Console.ReadLine();
        if (line != null && IsExitWord(line.Trim()))
            ExitRequested = true;
    }

    public static int ReadMenuChoice(string prompt, int min, int max)
    {
        return ReadMenuChoice(prompt, min, max, false);
    }

    // Returns -1 when the user presses Enter and emptyIsDefault is true.
    public static int ReadMenuChoice(string prompt, int min, int max, bool emptyIsDefault)
    {
        string? text = ReadLine(prompt);
        if (text == null)
            return 0;

        if (text.Length == 0 && emptyIsDefault)
            return -1;

        if (int.TryParse(text, out int choice) && choice >= min && choice <= max)
            return choice;

        return 0;
    }

    public static bool IsExitWord(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        string word = text.Trim().ToLower();
        return word is "exit" or "quit" or "q";
    }

    public static string? ReadText(string prompt, string? keepValue = null)
    {
        while (!ExitRequested)
        {
            string? text = ReadLine(prompt);
            if (text == null)
                return null;

            if (text.Length == 0 && keepValue != null)
                return keepValue;

            if (text.Length > 0)
                return text;

            ColoredText.WriteLine(">> Please type a title.", ConsoleColor.Red);
        }

        return null;
    }

    public static decimal? ReadAmount(string prompt, decimal? keepValue = null)
    {
        while (!ExitRequested)
        {
            string? text = ReadLine(prompt);
            if (text == null)
                return null;

            if (text.Length == 0 && keepValue != null)
                return keepValue;

            string normalized = text.Replace(" ", string.Empty).Replace(',', '.');
            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)
                && amount > 0)
                return amount;

            ColoredText.WriteLine(">> Please type an amount greater than 0. Example: 150.50", ConsoleColor.Red);
        }

        return null;
    }

    public static int? ReadMonth(string prompt, int? keepValue = null)
    {
        while (!ExitRequested)
        {
            string? text = ReadLine(prompt);
            if (text == null)
                return null;

            if (text.Length == 0 && keepValue != null)
                return keepValue;

            if (int.TryParse(text, out int month) && month >= 1 && month <= 12)
                return month;

            for (int index = 1; index < MoneyItem.MonthNames.Length; index++)
            {
                if (text.Equals(MoneyItem.MonthNames[index], StringComparison.OrdinalIgnoreCase))
                    return index;
            }

            ColoredText.WriteLine(">> Please type a month from 1 to 12, or a name like March.", ConsoleColor.Red);
        }

        return null;
    }

    public static MoneyKind? ReadKind(string prompt, MoneyKind? keepValue = null)
    {
        while (!ExitRequested)
        {
            string? text = ReadLine(prompt);
            if (text == null)
                return null;

            if (text.Length == 0 && keepValue != null)
                return keepValue;

            string word = text.ToLower();
            if (word is "1" or "income")
                return MoneyKind.Income;
            if (word is "2" or "expense")
                return MoneyKind.Expense;

            ColoredText.WriteLine(">> Type 1 for income or 2 for expense.", ConsoleColor.Red);
        }

        return null;
    }

    public static int ReadItemNumber(string prompt)
    {
        while (!ExitRequested)
        {
            string? text = ReadLine(prompt);
            if (text == null)
                return 0;

            if (int.TryParse(text, out int id) && id > 0)
                return id;

            ColoredText.WriteLine(">> Please type the item number. Example: 2", ConsoleColor.Red);
        }

        return 0;
    }

    public static bool? ReadYesNo(string prompt)
    {
        while (!ExitRequested)
        {
            string? text = ReadLine(prompt);
            if (text == null)
                return null;

            string word = text.ToLower();
            if (word is "y" or "yes")
                return true;
            if (word is "n" or "no")
                return false;

            ColoredText.WriteLine(">> Please type yes or no.", ConsoleColor.Red);
        }

        return null;
    }

    private static string? ReadLine(string prompt)
    {
        ColoredText.Write(prompt, ConsoleColor.Cyan);
        string text = Console.ReadLine()?.Trim() ?? string.Empty;

        if (IsExitWord(text))
        {
            ExitRequested = true;
            return null;
        }

        return text;
    }
}