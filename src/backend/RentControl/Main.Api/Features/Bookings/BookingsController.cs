using Main.Api.Features.Bookings.GetByStatus;
using Main.Api.Features.Bookings.Issue;
using Main.Api.Features.Bookings.Return;
using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Bookings.Get;
using Main.Api.Features.Bookings.GetPage;
using Main.Api.Features.Bookings.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using RentControl.Common.DTOs;
using Main.Model.Models;

namespace Main.Api.Features.Bookings;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IMediator mediator) : ControllerBase
{
    [HttpGet("by-status")]
    public async Task<ActionResult<List<BookingDto>>>  GetByStatusAsync(
      [FromQuery] BookingStatusEnum? bookingStatus,
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

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(
        [FromBody] CreateBookingRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<CreateBookingCommand>() with { Id = Guid.NewGuid() };
        var client = mediator.CreateRequestClient<CreateBookingCommand>();
        var response = await client.GetResponse<BookingDto>(command, ct);
        return Created($"api/bookings/{response.Message.Id}", response.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateBookingRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<UpdateBookingCommand>() with { Id = id };
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingDto>> Get(Guid id, CancellationToken ct)
    {
        var command = new GetBookingCommand(id);
        var client = mediator.CreateRequestClient<GetBookingCommand>();
        var response = await client.GetResponse<BookingDto>(command, ct);
        return Ok(response.Message);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<BookingDto>>> GetPage(
        [FromQuery] GetPageBookingRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<GetPageBookingCommand>();
        var client = mediator.CreateRequestClient<GetPageBookingCommand>();
        var response = await client.GetResponse<PagedResponse<BookingDto>>(command, ct);
        return Ok(response.Message);
    }
}
