using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Delete;
using Main.Api.Features.Products.Get;
using Main.Api.Features.Products.GetMaxAvailableDuration;
using Main.Api.Features.Products.GetPage;
using Main.Api.Features.Products.Update;
using Mapster;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using RentControl.Common.DTOs;

namespace Main.Api.Features.Products;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<CreateProductCommand>();

        var client = mediator.CreateRequestClient<CreateProductCommand>();
        var response = await client.GetResponse<ProductDto>(command, cancellationToken: ct);

        return Created($"/api/products/{response.Message.Id}", response.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id, CancellationToken ct)
    {
        var command = new GetProductCommand(id);

        var client = mediator.CreateRequestClient<GetProductCommand>();
        var response = await client.GetResponse<ProductDto>(command, cancellationToken: ct);

        return Ok(response.Message);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetPage(
        [FromQuery] GetPageProductRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<GetPageProductCommand>();

        var client = mediator.CreateRequestClient<GetPageProductCommand>();
        var response = await client.GetResponse<PagedResponse<ProductDto>>(
            command,
            cancellationToken: ct
        );

        return Ok(response.Message);
    }

    [HttpGet("{id:guid}/max-duration")]
    public async Task<IActionResult> GetMaxDuration(
        Guid id,
        [FromQuery] GetMaxAvailableDurationRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<GetMaxAvailableDurationCommand>() with { Id = id };

        var client = mediator.CreateRequestClient<GetMaxAvailableDurationCommand>();
        var response = await client.GetResponse<ProductMaxAvailableDuration>(command, ct);

        return Ok(response.Message);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteProductCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct
    )
    {
        var command = request.Adapt<UpdateProductCommand>() with { Id = id };

        await mediator.Send(command, ct);

        return NoContent();
    }
}
