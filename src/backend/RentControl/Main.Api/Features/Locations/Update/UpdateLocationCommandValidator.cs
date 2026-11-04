using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.Id).Id();
        RuleFor(x => x.Name).RequiredString(50);
        RuleFor(x => x.Address).RequiredString(255);
    }
}
