using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.ReadyForPickup;

public class ReadyForPickupBookingCommandValidator : AbstractValidator<ReadyForPickupBookingCommand>
{
    public ReadyForPickupBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
