using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

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
}