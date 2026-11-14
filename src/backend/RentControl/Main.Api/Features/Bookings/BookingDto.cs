using Main.Api.Features.ProductImages;
using Main.Model.Models;

namespace Main.Api.Features.Bookings;

public record BookingDto(
    Guid Id,
    string? LocationName,
    string? LocationAddress,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    decimal DepositAmount,
    BookingStatusEnum BookingStatus,
    DateTimeOffset StatusChangedAt,
    string? ProductName,
    Guid? ProductId,
    ProductImageDto? MainImage,
    DateTimeOffset? ActualReturnAt,
    decimal? TotalRentalCost,
    bool IsInvoiceCreated
);
