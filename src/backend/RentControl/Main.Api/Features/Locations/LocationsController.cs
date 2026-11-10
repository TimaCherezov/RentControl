using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Delete;
using Main.Api.Features.Locations.Get;
using Main.Api.Features.Locations.GetPage;
using Main.Api.Features.Locations.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using RentControl.Common.DTOs;

namespace Main.Api.Features.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationsController(IMediator mediator) : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<LocationDto>> Create(
		[FromBody] CreateLocationRequest request,
		CancellationToken ct)
	{
		var command = request.Adapt<CreateLocationCommand>();

		var client = mediator.CreateRequestClient<CreateLocationCommand>();
		var response = await client.GetResponse<LocationDto>(command, cancellationToken: ct);

		return Created($"/api/locations/{response.Message.Id}", response.Message);
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<LocationDto>> Get(Guid id, CancellationToken ct)
	{
		var command = new GetLocationCommand(id);

		var client = mediator.CreateRequestClient<GetLocationCommand>();
		var response = await client.GetResponse<LocationDto>(command, cancellationToken: ct);

		return Ok(response.Message);
	}

	[HttpGet]
	public async Task<ActionResult<PagedResponse<LocationDto>>> GetPage(
		[FromQuery] GetPageLocationRequest request,
		CancellationToken ct)
	{
		var command = request.Adapt<GetPageLocationCommand>();

		var client = mediator.CreateRequestClient<GetPageLocationCommand>();
		var response = await client.GetResponse<PagedResponse<LocationDto>>(command, cancellationToken: ct);

		return Ok(response.Message);
	}

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		Guid id,
		[FromBody] UpdateLocationRequest request,
		CancellationToken ct)
	{
		var command = request.Adapt<UpdateLocationCommand>() with { Id = id };

		await mediator.Send(command, ct);

		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
	{
		var command = new DeleteLocationCommand(id);
		await mediator.Send(command, ct);
		return NoContent();
	}
}
