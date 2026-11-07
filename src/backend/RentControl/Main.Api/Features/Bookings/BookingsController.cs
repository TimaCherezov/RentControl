using Main.Api.Features.Bookings.Cancel;
using Main.Api.Features.Bookings.Complete;
using Main.Api.Features.Bookings.Confirm;
using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Bookings.Get;
using Main.Api.Features.Bookings.GetPage;
using Main.Api.Features.Bookings.Issue;
using Main.Api.Features.Bookings.Overdue;
using Main.Api.Features.Bookings.ReadyForPickup;
using Main.Api.Features.Bookings.Return;
using Main.Api.Features.Bookings.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentControl.Common.DTOs;
using Main.Model.Models;

namespace Main.Api.Features.Bookings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/issuance")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsIssuedAsync(Guid id, CancellationToken ct)
    {
        var command = new IssueBookingCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/return")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsReturnedAsync(Guid id, CancellationToken ct)
    {
        var command = new ReturnBookingCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsConfirmedAsync(Guid id, CancellationToken ct)
    {
        var command = new ConfirmBookingCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsCancelledAsync(Guid id, CancellationToken ct)
    {
        var command = new CancelBookingCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/ready-for-pickup")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsReadyForPickupAsync(Guid id, CancellationToken ct)
    {
        var command = new ReadyForPickupBookingCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/overdue")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsOverdueAsync(Guid id, CancellationToken ct)
    {
        var command = new OverdueBookingCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> MarkAsCompletedAsync(Guid id, CancellationToken ct)
    {
        var command = new CompleteBookingCommand(id);
        await mediator.Send(command, ct);
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
