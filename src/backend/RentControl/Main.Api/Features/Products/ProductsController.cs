using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.Products;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var command = request.Adapt<CreateProductCommand>();

        var client = mediator.CreateRequestClient<CreateProductCommand>();
        var response = await client.GetResponse<ProductDto>(command, cancellationToken: ct);

        return Created($"/api/products/{response.Message.Id}", response.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        var command = request.Adapt<UpdateProductCommand>() with { Id = id };

        await mediator.Send(command, ct);

        return NoContent();
    }

}
