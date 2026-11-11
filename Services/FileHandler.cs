using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoneyTracker.Helpers;
using MoneyTracker.Models;

namespace MoneyTracker.Services;

// Reads and writes the money list.
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

    public bool Load(MoneyManager manager)
    {
        try
        {
            string path = FilePath(SaveFileName, createFolder: false);
            string json = File.ReadAllText(path);
            MoneyFile? file = JsonSerializer.Deserialize<MoneyFile>(json, JsonOptions());

            if (file == null)
            {
                ColoredText.WriteLine(">> Could not open the saved file. Starting with an empty list.", ConsoleColor.Red);
                return false;
            }

            List<MoneyItem> loaded = file.Items ?? new List<MoneyItem>();
            int skipped = manager.LoadItems(loaded, file.NextId);

            if (skipped > 0)
            {
                string rowWord = skipped == 1 ? "row" : "rows";
                ColoredText.WriteLine($">> Skipped {skipped} saved {rowWord} with a missing or repeated number.", ConsoleColor.Yellow);
            }

            int count = manager.GetItems(null).Count;
            ColoredText.WriteLine($">> Opened your saved list. {count} items.", ConsoleColor.Green);
            return true;
        }
        catch (Exception)
        {
            ColoredText.WriteLine(">> Could not open the saved file. Starting with an empty list.", ConsoleColor.Red);
            return false;
        }
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