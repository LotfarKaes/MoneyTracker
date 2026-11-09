using InPro1MoneyTracker.Models;

namespace InPro1MoneyTracker.Services;

// Reads and writes the money files. Nothing is saved yet.
public class FileHandler
{
    public const string DataFolderName = "Data";
    public const string SaveFileName = "money.json";
    public const string ReportFileName = "money-report.txt";
    public const string BackupFolderName = "Backups";

    public List<MoneyItem> Load()
    {
        // Step 9: read Data/money.json when the program starts.
        return new List<MoneyItem>();
    }

    public void Save(IReadOnlyList<MoneyItem> items, int nextId)
    {
        // Step 8: copy the old file into Backups, then write money.json.
    }

    public void WriteCleanReport(IReadOnlyList<MoneyItem> items)
    {
        // Step 8: write Data/money-report.txt in plain columns.
    }
}