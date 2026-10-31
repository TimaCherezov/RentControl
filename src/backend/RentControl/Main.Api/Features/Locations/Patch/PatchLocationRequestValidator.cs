using FluentValidation;

namespace Main.Api.Features.Locations.Patch;

public class PatchLocationRequestValidator : AbstractValidator<PatchLocationRequest>
{
	public PatchLocationRequestValidator()
	{
		RuleFor(x => x)
			.Must(HasAtLeastOneField)
			.WithMessage("At least one field must be provided");

		When(x => x.Name is not null, () =>
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.WithMessage("Name must not be empty")
				.MaximumLength(50)
				.WithMessage("Name must be at most 50 characters");
		});

		When(x => x.Address is not null, () =>
		{
			RuleFor(x => x.Address)
				.NotEmpty()
				.WithMessage("Address must not be empty")
				.MaximumLength(255)
				.WithMessage("Address must be at most 255 characters");
		});
	}
	
	private static bool HasAtLeastOneField(PatchLocationRequest request)
	{
		return request.Name is not null
		    || request.Address is not null;
	}
}