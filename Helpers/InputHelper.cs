namespace InPro1MoneyTracker.Helpers;

// Reads what the user types. Amounts, months, and yes/no answers come in later steps.
public static class InputHelper
{
    public static int ReadMenuChoice(string prompt, int min, int max)
    {
        ColoredText.Write(prompt, ConsoleColor.Cyan);
        string text = Console.ReadLine()?.Trim() ?? string.Empty;

        // exit, quit, and q do the same job as the last menu number (Save and Quit).
        if (IsExitWord(text))
            return max;

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

    public static decimal ReadAmount(string prompt)
    {
        // Step 7: accept 12.50 and 12,50. Reject zero and empty text.
        return 0m;
    }

    public static int ReadMonth(string prompt)
    {
        // Step 7: accept 3 or March, and keep the month as a number from 1 to 12.
        return 0;
    }

    public static bool ReadYesNo(string prompt)
    {
        // Step 6: ask before a row is removed.
        return false;
    }
}