using FluentValidation;
using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Create;
using Microsoft.Extensions.Options;

namespace RentControl.UnitTests.MainTests;

public class CreateBookingCommandValidatorTests
{
    private static readonly DateTimeOffset Tomorrow = DateTimeOffset.UtcNow.AddDays(1);
    private static readonly DateTimeOffset DayAfterTomorrow = Tomorrow.AddDays(1);

    private readonly CreateBookingCommandValidator _validator = new(
        Options.Create(new BookingOptions { MaxBookingDuration = TimeSpan.FromDays(1) }));

    [Fact]
    public void Validate_WhenProductIdIsEmpty_IsInvalid()
    {
        var command = new CreateBookingCommand(Guid.Empty, null, Tomorrow, DayAfterTomorrow, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenStartAtIsInPast_IsInvalid()
    {
        var command = new CreateBookingCommand(
            Guid.NewGuid(), null, DateTimeOffset.UtcNow.AddDays(-1), Tomorrow, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenStartAtIsAfterEndAt_IsInvalid()
    {
        var command = new CreateBookingCommand(
            Guid.NewGuid(), null, DayAfterTomorrow, Tomorrow, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenDurationExceedsMaxBookingDuration_IsInvalid()
    {
        var command = new CreateBookingCommand(
            Guid.NewGuid(), null, Tomorrow, Tomorrow.AddDays(3), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_Succeeds()
    {
        var command = new CreateBookingCommand(
            Guid.NewGuid(), null, Tomorrow, Tomorrow.AddHours(2), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
