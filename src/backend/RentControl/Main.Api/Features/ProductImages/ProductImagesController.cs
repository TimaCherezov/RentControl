using Main.Api.Features.ProductImages.Create;
using Main.Api.Features.ProductImages.Delete;
using Main.Api.Features.ProductImages.Get;
using Main.Api.Features.ProductImages.GetAll;
using Main.Api.Features.ProductImages.GetMain;
using Main.Api.Features.ProductImages.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.ProductImages;

[ApiController]
[Route("api/[controller]")]
public class ProductImagesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductImageDto>> Create(
        [FromBody] CreateProductImageRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<CreateProductImageCommand>();

        var client = mediator.CreateRequestClient<CreateProductImageCommand>();
        var response = await client.GetResponse<ProductImageDto>(command, cancellationToken: ct);

        return Created($"/api/productimages/{response.Message.Id}", response.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductImageDto>> Get(Guid id, CancellationToken ct)
    {
        var command = new GetProductImageCommand(id);

        var client = mediator.CreateRequestClient<GetProductImageCommand>();
        var response = await client.GetResponse<ProductImageDto>(command, cancellationToken: ct);

        return Ok(response.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductImageRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<UpdateProductImageCommand>() with { Id = id };

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteProductImageCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<List<ProductImageDto>>> GetAll(
        Guid productId,
        CancellationToken ct
    )
    {
        var command = new GetAllProductImageCommand(productId);
        var client = mediator.CreateRequestClient<GetAllProductImageCommand>();
        var response = await client.GetResponse<List<ProductImageDto>>(
            command,
            cancellationToken: ct
        );
        return Ok(response.Message);
    }

    [HttpGet("product/{productId:guid}/main")]
    public async Task<ActionResult<ProductImageDto>> GetMain(Guid productId, CancellationToken ct)
    {
        var command = new GetMainProductImageCommand(productId);
        var client = mediator.CreateRequestClient<GetMainProductImageCommand>();
        var response = await client.GetResponse<ProductImageDto>(command, cancellationToken: ct);
        return Ok(response.Message);
    }
}
