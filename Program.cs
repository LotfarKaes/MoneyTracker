using InPro1MoneyTracker.Helpers;
using InPro1MoneyTracker.Models;

// Step 3 creates one MoneyItem so we can see the five fields.
// The list of many items comes in step 4. The menu comes in step 5.

Console.OutputEncoding = System.Text.Encoding.UTF8;

ColoredText.WriteLine(">> Welcome to TrackMoney", ConsoleColor.Cyan);

MoneyItem salary = new MoneyItem(1, "Salary", 25000m, 1, MoneyKind.Income);

ColoredText.WriteLine(">> One sample item is ready:", ConsoleColor.Green);

ConsoleColor rowColor = salary.Kind == MoneyKind.Income
    ? ConsoleColor.Green
    : ConsoleColor.Red;

ColoredText.WriteLine(">> " + salary, rowColor);
ColoredText.WriteLine(">> The list and the menu come in the next steps.", ConsoleColor.Yellow);