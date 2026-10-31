using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Patch;

public class PatchLocationRequestValidator : AbstractValidator<PatchLocationRequest>
{
    public PatchLocationRequestValidator()
    {
        RuleFor(x => x)
            .Must(HasAtLeastOneField)
            .WithMessage("At least one field must be provided");

        this.AddOptionalNonEmptyStringRules(x => x.Name, "Name", 50);

        this.AddOptionalNonEmptyStringRules(x => x.Address, "Address", 255);
    }

    private static bool HasAtLeastOneField(PatchLocationRequest request)
    {
        return request.Name is not null
            || request.Address is not null;
    }
}
