namespace Main.Api.Features.Products.Patch;

public record PatchProductCommand(
    Guid Id,
    string? Name,
    decimal? MarketValue,
    decimal? DepositOverride,
    decimal? DailyRate,
    decimal? HourlyRate,
    decimal? WeeklyRate,
    decimal? MonthlyRate,
    string? Description
);