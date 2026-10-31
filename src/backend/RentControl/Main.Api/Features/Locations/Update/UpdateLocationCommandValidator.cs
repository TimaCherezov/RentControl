using FluentValidation;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
	public UpdateLocationCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Id is required");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Name is required")
			.MaximumLength(50)
			.WithMessage("Name must be at most 50 characters");

		RuleFor(x => x.Address)
			.NotEmpty()
			.WithMessage("Address is required")
			.MaximumLength(255)
			.WithMessage("Address must be at most 255 characters");
	}
}