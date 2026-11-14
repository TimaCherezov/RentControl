using Main.Api.Features.ProductImages;

namespace Main.Api.Features.Products;

public record ProductDto(
    Guid Id,
    string Name,
    decimal MarketValue,
    decimal? DepositOverride,
    decimal DepositAmount,
    decimal DailyRate,
    decimal HourlyRate,
    decimal? WeeklyRate,
    decimal? MonthlyRate,
    string? Description,
    List<ProductImageDto> Images
);
