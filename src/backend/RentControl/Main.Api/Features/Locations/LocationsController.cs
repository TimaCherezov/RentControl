using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Delete;
using Main.Api.Features.Locations.Get;
using Main.Api.Features.Locations.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Main.Model.Models;

namespace Main.Api.Features.Locations;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(RoleEnum.Admin))]
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
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
	{
		var command = new DeleteLocationCommand(id);
		await mediator.Send(command, ct);
		return NoContent();
	}
}
