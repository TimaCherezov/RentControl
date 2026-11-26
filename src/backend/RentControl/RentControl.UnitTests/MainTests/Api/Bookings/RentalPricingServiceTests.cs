using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Pricing;
using Microsoft.Extensions.Options;
using Main.Model.Models;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class RentalPricingServiceTests
{
    private static readonly DateTimeOffset StartAt = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);

    private static RentalPricingService CreateService(decimal depositRate = 0.2m) =>
        new(Options.Create(new BookingOptions { DepositRate = depositRate }));

    private static BookingQuote Calculate(Product product, int durationHours) =>
        CreateService().Calculate(product, StartAt, StartAt.AddHours(durationHours));

    [Fact]
    public void Calculate_WhenEndNotAfterStart_ThrowsInvalidOperationException()
    {
        var product = new ProductBuilder().Build();

        Assert.Throws<InvalidOperationException>(
            () => CreateService().Calculate(product, StartAt, StartAt));
    }

    [Fact]
    public void Calculate_WhenHalfHourPeriod_BillsMinimumOneHour()
    {
        var product = new ProductBuilder()
            .WithHourlyRate(100m)
            .WithDailyRate(1000m)
            .Build();

        var quote = CreateService().Calculate(product, StartAt, StartAt.AddMinutes(30));

        var line = Assert.Single(quote.Lines);
        Assert.Equal("hour", line.Unit);
        Assert.Equal(1, line.Count);
        Assert.Equal(100m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenPartialHours_RoundsUpToNextHour()
    {
        var product = new ProductBuilder()
            .WithHourlyRate(100m)
            .WithDailyRate(1000m)
            .Build();

        var quote = CreateService().Calculate(product, StartAt, StartAt.AddHours(2.5));

        Assert.Equal(300m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenDurationIsExactlyOneDay_BillsSingleDayLine()
    {
        var product = new ProductBuilder()
            .WithHourlyRate(100m)
            .WithDailyRate(1000m)
            .Build();

        var quote = Calculate(product, 24);

        var line = Assert.Single(quote.Lines);
        Assert.Equal("day", line.Unit);
        Assert.Equal(1, line.Count);
        Assert.Equal(1000m, line.Subtotal);
        Assert.Equal(1000m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenDurationHasDayRemainder_DecomposesToHours()
    {
        var product = new ProductBuilder()
            .WithHourlyRate(100m)
            .WithDailyRate(1000m)
            .Build();

        var quote = Calculate(product, 50);

        Assert.Collection(
            quote.Lines,
            line =>
            {
                Assert.Equal("day", line.Unit);
                Assert.Equal(2, line.Count);
                Assert.Equal(2000m, line.Subtotal);
            },
            line =>
            {
                Assert.Equal("hour", line.Unit);
                Assert.Equal(2, line.Count);
                Assert.Equal(200m, line.Subtotal);
            });
        Assert.Equal(2200m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenAllRatesSet_DecomposesMonthsWeeksDaysHours()
    {
        var product = new ProductBuilder()
            .WithMonthlyRate(7200m)
            .WithWeeklyRate(1680m)
            .WithDailyRate(240m)
            .WithHourlyRate(10m)
            .Build();

        var quote = Calculate(product, 1000);

        Assert.Collection(
            quote.Lines,
            line =>
            {
                Assert.Equal("month", line.Unit);
                Assert.Equal(1, line.Count);
                Assert.Equal(7200m, line.Subtotal);
            },
            line =>
            {
                Assert.Equal("week", line.Unit);
                Assert.Equal(1, line.Count);
                Assert.Equal(1680m, line.Subtotal);
            },
            line =>
            {
                Assert.Equal("day", line.Unit);
                Assert.Equal(4, line.Count);
                Assert.Equal(960m, line.Subtotal);
            },
            line =>
            {
                Assert.Equal("hour", line.Unit);
                Assert.Equal(16, line.Count);
                Assert.Equal(160m, line.Subtotal);
            });
        Assert.Equal(10000m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenWeeklyRateMissing_SkipsWeekUnit()
    {
        var product = new ProductBuilder()
            .WithMonthlyRate(7200m)
            .WithWeeklyRate(null)
            .WithDailyRate(240m)
            .WithHourlyRate(10m)
            .Build();

        var quote = Calculate(product, 900);

        Assert.DoesNotContain(quote.Lines, line => line.Unit == "week");
        Assert.Collection(
            quote.Lines,
            line => Assert.Equal("month", line.Unit),
            line => Assert.Equal("day", line.Unit),
            line => Assert.Equal("hour", line.Unit));
        Assert.Equal(9000m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenMonthlyRateMissing_SkipsMonthUnit()
    {
        var product = new ProductBuilder()
            .WithMonthlyRate(null)
            .WithWeeklyRate(1680m)
            .WithDailyRate(240m)
            .WithHourlyRate(10m)
            .Build();

        var quote = Calculate(product, 1000);

        Assert.DoesNotContain(quote.Lines, line => line.Unit == "month");
        Assert.Equal(10000m, quote.RentalCost);
    }

    [Fact]
    public void Calculate_WhenDepositOverrideSet_TotalIncludesOverrideDeposit()
    {
        var product = new ProductBuilder()
            .WithMarketValue(1000m)
            .WithDepositOverride(150m)
            .WithHourlyRate(100m)
            .Build();

        var quote = Calculate(product, 1);

        Assert.Equal(150m, quote.DepositAmount);
        Assert.Equal(250m, quote.TotalAmount);
    }

    [Fact]
    public void Calculate_WhenNoDepositOverride_ComputesDepositFromMarketValue()
    {
        var product = new ProductBuilder()
            .WithMarketValue(1000m)
            .WithHourlyRate(100m)
            .Build();

        var quote = Calculate(product, 1);

        Assert.Equal(200m, quote.DepositAmount);
        Assert.Equal(300m, quote.TotalAmount);
    }

    [Fact]
    public void Calculate_WhenValidPeriod_SetsQuotePeriodAndDuration()
    {
        var product = new ProductBuilder().Build();
        var endAt = StartAt.AddHours(1);

        var quote = CreateService().Calculate(product, StartAt, endAt);

        Assert.Equal(product.Id, quote.ProductId);
        Assert.Equal(StartAt, quote.StartAt);
        Assert.Equal(endAt, quote.EndAt);
        Assert.Equal(3600, quote.DurationSeconds);
    }
}
