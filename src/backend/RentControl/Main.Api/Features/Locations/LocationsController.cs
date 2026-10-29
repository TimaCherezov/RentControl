using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Patch;
using Main.Api.Features.Locations.Update;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationsController(IServiceProvider serviceProvider, IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create(
        [FromBody] CreateLocationRequest request,
        CancellationToken ct)
    {
        var command = new CreateLocationCommand(request.Name!, request.Address!);

        var client = serviceProvider.CreateRequestClient<CreateLocationCommand>();
        var response = await client.GetResponse<LocationDto>(command, cancellationToken: ct);

        return Created($"/api/locations/{response.Message.Id}", response.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken ct)
    {
        var command = new UpdateLocationCommand(id, request.Name!, request.Address!);

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(
        Guid id,
        [FromBody] PatchLocationRequest request,
        CancellationToken ct)
    {
        var command = new PatchLocationCommand(id, request.Name, request.Address);

        await mediator.Send(command, ct);

        return NoContent();
    }
}
