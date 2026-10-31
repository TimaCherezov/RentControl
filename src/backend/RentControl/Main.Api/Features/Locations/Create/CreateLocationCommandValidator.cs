using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Create;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
	public CreateLocationCommandValidator()
	{
		this.AddRequiredStringRules(x => x.Name, "Name", 50);

		this.AddRequiredStringRules(x => x.Address, "Address", 255);
	}
}
