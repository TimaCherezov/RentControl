using FluentValidation;
using Main.Api.Features.Locations.Patch;

namespace Main.Api.Features.Locations;

public class PatchLocationValidator : AbstractValidator<PatchLocationRequest>
{
    public PatchLocationValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50).When(x => x.Name is not null)
            .WithMessage("Name must be at most 50 characters");

        RuleFor(x => x.Address)
            .MaximumLength(255).When(x => x.Address is not null)
            .WithMessage("Address must be at most 255 characters");
    }
}
