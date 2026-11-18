using Main.Api.Features.ProductUnits.Create;
using Main.Api.Features.ProductUnits.Delete;
using Main.Api.Features.ProductUnits.Get;
using Main.Api.Features.ProductUnits.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.ProductUnits;

[ApiController]
[Route("api/[controller]")]
public class ProductUnitsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductUnitDto>> Create(
        [FromBody] CreateProductUnitRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<CreateProductUnitCommand>();

        var client = mediator.CreateRequestClient<CreateProductUnitCommand>();
        var response = await client.GetResponse<ProductUnitDto>(command, cancellationToken: ct);

        return Created($"/api/productunits/{response.Message.Id}", response.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductUnitDto>> Get(Guid id, CancellationToken ct)
    {
        var command = new GetProductUnitCommand(id);

        var client = mediator.CreateRequestClient<GetProductUnitCommand>();
        var response = await client.GetResponse<ProductUnitDto>(command, cancellationToken: ct);

        return Ok(response.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductUnitRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<UpdateProductUnitCommand>() with { Id = id };

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteProductUnitCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }
}
