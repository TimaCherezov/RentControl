namespace Main.Model.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal MarketValue { get; set; }
    public decimal? DepositOverride { get; set; }
    public decimal DailyRate { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal? WeeklyRate { get; set; }
    public decimal? MonthlyRate { get; set; }
    public string? Description { get; set; }
}
