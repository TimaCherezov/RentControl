using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Patch;

public class PatchLocationCommandValidator : AbstractValidator<PatchLocationCommand>
{
    public PatchLocationCommandValidator()
    {
        this.AddRequiredIdRules(x => x.Id, "Id");

        RuleFor(x => x)
            .Must(HasAtLeastOneField)
            .WithMessage("At least one field must be provided");

        this.AddOptionalNonEmptyStringRules(x => x.Name, "Name", 50);

        this.AddOptionalNonEmptyStringRules(x => x.Address, "Address", 255);
    }

    private static bool HasAtLeastOneField(PatchLocationCommand command)
    {
        return command.Name is not null
            || command.Address is not null;
    }
}
