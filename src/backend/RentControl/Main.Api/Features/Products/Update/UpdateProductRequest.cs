namespace Main.Api.Features.Products.Update;

public record UpdateProductRequest(
    string Name,
    decimal MarketValue,
    decimal? DepositOverride,
    decimal DailyRate,
    decimal HourlyRate,
    decimal? WeeklyRate,
    decimal? MonthlyRate,
    string? Description);
