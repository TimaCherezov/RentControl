namespace Main.Api.Features.Products.Update;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal MarketValue,
    decimal? DepositOverride,
    decimal DailyRate,
    decimal HourlyRate,
    decimal? WeeklyRate,
    decimal? MonthlyRate,
    string? Description
);
