using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Create;

public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
{
	public CreateLocationRequestValidator()
	{
		this.AddRequiredStringRules(x => x.Name, "Name", 50);

		this.AddRequiredStringRules(x => x.Address, "Address", 255);
	}
}
