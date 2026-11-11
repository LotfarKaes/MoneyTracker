using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoneyTracker.Helpers;
using MoneyTracker.Models;

namespace MoneyTracker.Services;

// Writes the money list to disk. Reading it back comes in the next step.
public class FileHandler
{
    public const string DataFolderName = "Data";
    public const string SaveFileName = "money.json";
    public const string ReportFileName = "money-report.txt";
    public const string BackupFolderName = "Backups";

    public bool SaveFileExists()
    {
        return File.Exists(FilePath(SaveFileName, createFolder: false));
    }

    public List<MoneyItem> Load()
    {
        // Step 9: read Data/money.json when the program starts.
        return new List<MoneyItem>();
    }

    public void Save(IReadOnlyList<MoneyItem> items, int nextId)
    {
        try
        {
            string savePath = FilePath(SaveFileName, createFolder: true);
            bool hadOldFile = File.Exists(savePath);

            if (hadOldFile)
                KeepBackup(savePath);

            MoneyFile file = new MoneyFile
            {
                NextId = nextId,
                Items = new List<MoneyItem>(items)
            };

            string json = JsonSerializer.Serialize(file, JsonOptions());
            File.WriteAllText(savePath, json);

            ColoredText.WriteLine($">> Saved {items.Count} items to Data/{SaveFileName}.", ConsoleColor.Green);

            if (hadOldFile)
                ColoredText.WriteLine(">> Kept a backup of the previous file in Data/Backups.", ConsoleColor.Yellow);
        }
        catch (Exception)
        {
            ColoredText.WriteLine(">> Could not save the money file.", ConsoleColor.Red);
        }
    }

    public void WriteCleanReport(IReadOnlyList<MoneyItem> items)
    {
        try
        {
            string reportPath = FilePath(ReportFileName, createFolder: true);
            List<string> lines = new List<string>
            {
                "Id".PadRight(6) + "Kind".PadRight(12) + "Month".PadRight(14) + "Title".PadRight(18) + "Amount"
            };

            if (items.Count == 0)
            {
                lines.Add("No items saved.");
            }
            else
            {
                foreach (MoneyItem item in items)
                {
                    string amount = item.Amount.ToString("0.00", CultureInfo.InvariantCulture) + " kr";
                    lines.Add(
                        item.Id.ToString().PadRight(6)
                        + item.Kind.ToString().PadRight(12)
                        + item.MonthName.PadRight(14)
                        + item.Title.PadRight(18)
                        + amount);
                }
            }

            File.WriteAllLines(reportPath, lines);
            ColoredText.WriteLine($">> Wrote a clean report to Data/{ReportFileName}.", ConsoleColor.Green);
        }
        catch (Exception)
        {
            ColoredText.WriteLine(">> Could not write the clean report.", ConsoleColor.Red);
        }
    }

    private static void KeepBackup(string savePath)
    {
        string? dataFolder = Path.GetDirectoryName(savePath);
        if (string.IsNullOrEmpty(dataFolder))
            throw new IOException("Could not find the data folder.");

        string backupFolder = Path.Combine(dataFolder, BackupFolderName);
        Directory.CreateDirectory(backupFolder);

        string stamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
        string backupPath = Path.Combine(backupFolder, $"money-{stamp}.json");
        int extra = 2;

        while (File.Exists(backupPath))
        {
            backupPath = Path.Combine(backupFolder, $"money-{stamp}-{extra}.json");
            extra++;
        }

        File.Copy(savePath, backupPath);
    }

    private static string FilePath(string fileName, bool createFolder)
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), DataFolderName);
        if (createFolder)
            Directory.CreateDirectory(folder);

        return Path.Combine(folder, fileName);
    }

    private static JsonSerializerOptions JsonOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private class MoneyFile
    {
        public int NextId { get; set; }
        public List<MoneyItem> Items { get; set; } = new List<MoneyItem>();
    }
}