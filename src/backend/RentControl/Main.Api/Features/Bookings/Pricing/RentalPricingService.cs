using Main.Model.Models;
using Microsoft.Extensions.Options;

namespace Main.Api.Features.Bookings.Pricing;

public sealed class RentalPricingService(IOptions<BookingOptions> options)
    : IRentalPricingService
{
    public BookingQuote Calculate(
        Product product,
        DateTimeOffset startAt,
        DateTimeOffset endAt)
    {
        var duration = endAt - startAt;
        if (duration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                "EndAt must be greater than StartAt.");
        }

        var billableHours = Math.Max(1, (int)Math.Ceiling(duration.TotalHours));

        var units = new List<(string Unit, int Hours, decimal Rate)>();

        if (product.MonthlyRate is { } monthlyRate)
        {
            units.Add(("month", 720, monthlyRate));
        }

        if (product.WeeklyRate is { } weeklyRate)
        {
            units.Add(("week", 168, weeklyRate));
        }

        units.Add(("day", 24, product.DailyRate));
        units.Add(("hour", 1, product.HourlyRate));

        var lines = new List<BookingQuoteLine>();
        var rentalCost = 0m;
        var remainingHours = billableHours;

        foreach (var unit in units)
        {
            var count = remainingHours / unit.Hours;

            if (count == 0)
            {
                continue;
            }

            lines.Add(
                new BookingQuoteLine(
                    unit.Unit,
                    count,
                    unit.Rate,
                    unit.Rate * count));

            rentalCost += unit.Rate * count;
            remainingHours -= count * unit.Hours;
        }

        var depositAmount = product.CalculateDeposit(options.Value.DepositRate);

        return new BookingQuote(
            product.Id,
            startAt,
            endAt,
            (long)duration.TotalSeconds,
            rentalCost,
            depositAmount,
            rentalCost + depositAmount,
            lines);
    }
}