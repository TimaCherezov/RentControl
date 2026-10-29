using FluentValidation;
using Main.Api.Features.Locations.Update;

namespace Main.Api.Features.Locations;

public class UpdateLocationValidator : AbstractValidator<UpdateLocationRequest>
{
    public UpdateLocationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(255).WithMessage("Address must be at most 255 characters");
    }
}
