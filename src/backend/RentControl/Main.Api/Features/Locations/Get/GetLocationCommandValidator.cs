using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Get;

public class GetLocationCommandValidator : AbstractValidator<GetLocationCommand>
{
    public GetLocationCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
