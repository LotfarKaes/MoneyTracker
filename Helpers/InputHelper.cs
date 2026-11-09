namespace InPro1MoneyTracker.Helpers;

// Reads what the user types. The checks for bad input come in step 7.
public static class InputHelper
{
    public static int ReadMenuChoice(string prompt, int min, int max)
    {
        // Step 5 reads a menu number. Step 7 asks again when the number is outside the range.
        return 0;
    }

    public static bool IsExitWord(string text)
    {
        // Step 5: exit, quit, and q count, in any mix of capital letters.
        return false;
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