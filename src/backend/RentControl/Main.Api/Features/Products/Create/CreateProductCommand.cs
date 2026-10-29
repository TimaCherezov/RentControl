namespace Main.Api.Features.Products.Create;

public record CreateProductCommand(
    string Name,
    decimal MarketValue,
    decimal? DepositOverride,
    decimal DailyRate,
    decimal HourlyRate,
    decimal? WeeklyRate,
    decimal? MonthlyRate,
    string? Description
);
