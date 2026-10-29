namespace Main.Api.Features.Products;

public record ProductDto(
    Guid Id,
    string Name,
    decimal MarketValue,
    decimal? DepositOverride,
    decimal DailyRate,
    decimal HourlyRate,
    decimal? WeeklyRate,
    decimal? MonthlyRate,
    string? Description);
