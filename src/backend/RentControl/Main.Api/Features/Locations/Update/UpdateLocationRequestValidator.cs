using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
{
	public UpdateLocationRequestValidator()
	{
		this.AddRequiredStringRules(x => x.Name, "Name", 50);

		this.AddRequiredStringRules(x => x.Address, "Address", 255);
	}
}
