using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Create;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
	public CreateLocationCommandValidator()
	{
		RuleFor(x => x.Name).RequiredString(50);
		RuleFor(x => x.Address).RequiredString(255);
	}
}