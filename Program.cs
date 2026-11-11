using MoneyTracker.Helpers;
using MoneyTracker.Services;

// Starts TrackMoney, loads the saved list, then hands the screen to the menu.

Console.OutputEncoding = System.Text.Encoding.UTF8;

MoneyManager manager = new MoneyManager();
FileHandler files = new FileHandler();

if (!files.SaveFileExists())
    ColoredText.WriteLine(">> No saved file yet. Starting with an empty list.", ConsoleColor.Yellow);
else
    files.Load(manager);

Menu menu = new Menu();
menu.Run(manager);