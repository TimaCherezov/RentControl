namespace Main.Api.Features.Bookings.Pricing;

public sealed record BookingQuoteLine(
    string Unit,
    int Count,
    decimal Rate,
    decimal Subtotal);

public sealed record BookingQuote(
    Guid ProductId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    long DurationSeconds,
    decimal RentalCost,
    decimal DepositAmount,
    decimal TotalAmount,
    IReadOnlyList<BookingQuoteLine> Lines);