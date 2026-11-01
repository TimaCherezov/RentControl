namespace Main.Api.Features.Products.Create;

public sealed class CreateProductRequest
{
    public string? Name { get; init; }
    public decimal? MarketValue { get; init; }
    public decimal? DepositOverride { get; init; }
    public decimal? DailyRate { get; init; }
    public decimal? HourlyRate { get; init; }
    public decimal? WeeklyRate { get; init; }
    public decimal? MonthlyRate { get; init; }
    public string? Description { get; init; }
}
