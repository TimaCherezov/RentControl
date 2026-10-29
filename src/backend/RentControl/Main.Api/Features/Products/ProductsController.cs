using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Patch;
using Main.Api.Features.Products.Update;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Main.Api.Features.Products;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IServiceProvider serviceProvider, IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var command = new CreateProductCommand(
            request.Name!,
            request.MarketValue ?? 0m,
            request.DepositOverride,
            request.DailyRate ?? 0m,
            request.HourlyRate ?? 0m,
            request.WeeklyRate,
            request.MonthlyRate,
            request.Description);

        var client = serviceProvider.CreateRequestClient<CreateProductCommand>();
        var response = await client.GetResponse<ProductDto>(command, cancellationToken: ct);

        return Created($"/api/products/{response.Message.Id}", response.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name!,
            request.MarketValue ?? 0m,
            request.DepositOverride,
            request.DailyRate ?? 0m,
            request.HourlyRate ?? 0m,
            request.WeeklyRate,
            request.MonthlyRate,
            request.Description);

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(
        Guid id,
        [FromBody] PatchProductRequest request,
        CancellationToken ct)
    {
        var command = new PatchProductCommand(
            id,
            request.Name,
            request.MarketValue,
            request.DepositOverride,
            request.DailyRate,
            request.HourlyRate,
            request.WeeklyRate,
            request.MonthlyRate,
            request.Description);

        await mediator.Send(command, ct);

        return NoContent();
    }
}
