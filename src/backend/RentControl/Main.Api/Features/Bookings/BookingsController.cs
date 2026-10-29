using Main.Api.Features.Bookings.Commands;
using Main.Api.Features.Bookings.Dto;
using Main.Api.Features.Bookings.Queries;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.Bookings;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IServiceProvider serviceProvider, IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> Get(
        [FromQuery] string? status,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            var client = serviceProvider.CreateRequestClient<GetBookingsQuery>();
            var response = await client.GetResponse<BookingsListResponse>(new GetBookingsQuery(), cancellationToken: ct);
            return Ok(response.Message.Bookings);
        }

        var statusClient = serviceProvider.CreateRequestClient<GetBookingsByStatusQuery>();
        var statusResponse = await statusClient.GetResponse<BookingsListResponse>(
            new GetBookingsByStatusQuery(status), cancellationToken: ct);
        return Ok(statusResponse.Message.Bookings);
    }

    [HttpPost("{id:guid}/issuance")]
    public async Task<IActionResult> Issue(Guid id, CancellationToken ct)
    {
        var command = new IssueBookingCommand(id);

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id, CancellationToken ct)
    {
        var command = new ReturnBookingCommand(id);

        await mediator.Send(command, ct);

        return NoContent();
    }
}
