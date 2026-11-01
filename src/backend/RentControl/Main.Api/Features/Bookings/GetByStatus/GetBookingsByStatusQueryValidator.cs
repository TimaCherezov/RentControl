using FluentValidation;
using Main.Model.Models;

namespace Main.Api.Features.Bookings.GetByStatus;

public class GetBookingsByStatusQueryValidator : AbstractValidator<GetBookingsByStatusQuery>
{
	public GetBookingsByStatusQueryValidator()
	{
		RuleFor(x => x.Status)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Status is required.")
			.Must(BeValidStatus)
			.WithMessage("Invalid booking status.");
	}

	private static bool BeValidStatus(string? status)
	{
		return Enum.TryParse<BookingStatusEnum>(status, true, out var result)
		       && Enum.IsDefined(result)
		       && !int.TryParse(status, out _);
	}
}