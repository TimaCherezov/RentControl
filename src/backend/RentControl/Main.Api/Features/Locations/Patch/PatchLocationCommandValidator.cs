using FluentValidation;

namespace Main.Api.Features.Locations.Patch;

public class PatchLocationCommandValidator : AbstractValidator<PatchLocationCommand>
{
	public PatchLocationCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Id is required");

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
	
	private static bool HasAtLeastOneField(PatchLocationCommand command)
	{
		return command.Name is not null
		       || command.Address is not null;
	}
}