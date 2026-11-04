using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Delete;

public class DeleteLocationCommandValidator : AbstractValidator<DeleteLocationCommand>
{
    public DeleteLocationCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
