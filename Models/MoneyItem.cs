using System.Text.Json.Serialization;

namespace MoneyTracker.Models;

// One row in the tracker.
// The amount is always stored as a positive number. Kind says if it is income or expense.
public class MoneyItem
{
    // Index 0 is empty so that month number 1 lines up with January.
    public static readonly string[] MonthNames =
    {
        "",
        "January", "February", "March", "April",
        "May", "June", "July", "August",
        "September", "October", "November", "December"
    };

    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public MoneyKind Kind { get; set; }

    public MoneyItem(int id, string title, decimal amount, int month, MoneyKind kind)
    {
        Id = id;
        Title = title;
        Amount = amount;
        Month = month;
        Kind = kind;
    }

    [JsonIgnore]
    public string MonthName =>
        Month >= 1 && Month <= 12 ? MonthNames[Month] : Month.ToString();

    public override string ToString()
    {
        return $"#{Id}  {Kind}  {MonthName}  {Title}  {Amount:0.00} kr";
    }
}