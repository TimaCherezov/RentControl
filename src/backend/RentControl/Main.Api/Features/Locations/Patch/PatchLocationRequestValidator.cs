using FluentValidation;

namespace Main.Api.Features.Locations.Patch;

public class PatchLocationRequestValidator : AbstractValidator<PatchLocationRequest>
{
	public PatchLocationRequestValidator()
	{
		RuleFor(x => x)
			.Must(HasAtLeastOneField)
			.WithMessage("At least one field must be provided");

		RuleFor(x => x.Name)
			.NotEmpty()
			.When(x => x.Name is not null)
			.WithMessage("Name cannot be empty");

		RuleFor(x => x.Name)
			.MaximumLength(50)
			.When(x => x.Name is not null)
			.WithMessage("Name must be at most 50 characters");

		RuleFor(x => x.Address)
			.NotEmpty()
			.When(x => x.Address is not null)
			.WithMessage("Address cannot be empty");

		RuleFor(x => x.Address)
			.MaximumLength(255)
			.When(x => x.Address is not null)
			.WithMessage("Address must be at most 255 characters");
	}

	private static bool HasAtLeastOneField(PatchLocationRequest request)
	{
		return request.Name is not null
		    || request.Address is not null;
	}
}