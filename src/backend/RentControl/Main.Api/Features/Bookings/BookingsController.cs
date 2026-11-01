using Main.Api.Features.Bookings.Commands;
using Main.Api.Features.Bookings.Dto;
using Main.Api.Features.Bookings.Queries;
using Main.Model.Models;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.Bookings;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IServiceProvider serviceProvider, IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<BookingDto>>> Get([FromQuery] string status, CancellationToken ct)
	{
		if (!Enum.TryParse<BookingStatusEnum>(status, true, out var bookingStatus)
		    || !Enum.IsDefined(bookingStatus))
			return BadRequest($"Invalid booking status: {status}");

		var client = serviceProvider.CreateRequestClient<GetBookingsByStatusQuery>();
		var response = await client.GetResponse<BookingsListResponse>(
			new GetBookingsByStatusQuery(bookingStatus),
			cancellationToken: ct);

		return Ok(response.Message.Bookings);
	}

	[HttpPost("{id:guid}/issuance")]
	public async Task<IActionResult> Issue(Guid id, CancellationToken ct)
	{
		await mediator.Send(new IssueBookingCommand(id), ct);

		return NoContent();
	}

	[HttpPost("{id:guid}/return")]
	public async Task<IActionResult> Return(Guid id, CancellationToken ct)
	{
		await mediator.Send(new ReturnBookingCommand(id), ct);

		return NoContent();
	}
}