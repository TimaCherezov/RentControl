using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
{
	public UpdateLocationRequestValidator()
	{
		RuleFor(x => x.Name).RequiredString(50);
		RuleFor(x => x.Address).RequiredString(255);
	}
}