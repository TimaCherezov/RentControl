using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
	public UpdateLocationCommandValidator()
	{
		this.AddRequiredIdRules(x => x.Id, "Id");

		this.AddRequiredStringRules(x => x.Name, "Name", 50);

		this.AddRequiredStringRules(x => x.Address, "Address", 255);
	}
}
