using Main.Api.Features.Bookings.GetByStatus;
using Main.Api.Features.Bookings.Issue;
using Main.Api.Features.Bookings.Return;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.Bookings;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IMediator mediator) : ControllerBase
{
	[HttpGet("by-status")]
	public async Task<ActionResult<List<BookingDto>>>  GetByStatusAsync(
		[FromQuery] string? bookingStatus,
		CancellationToken ct)
	{
		var client = mediator.CreateRequestClient<GetBookingsByStatusQuery>();
		var response = await client.GetResponse<BookingsListResponse>(
			new GetBookingsByStatusQuery(bookingStatus),
			cancellationToken: ct);

		return Ok(response.Message.Bookings);
	}

	[HttpPost("{id:guid}/issuance")]
	public async Task<IActionResult> MarkAsIssuedAsync(Guid id, CancellationToken ct)
	{
		await mediator.Send(new IssueBookingCommand(id), ct);
		return NoContent();
	}

	[HttpPost("{id:guid}/return")]
	public async Task<IActionResult> MarkAsReturnedAsync(Guid id, CancellationToken ct)
	{
		await mediator.Send(new ReturnBookingCommand(id), ct);
		return NoContent();
	}
}