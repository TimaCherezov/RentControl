using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.Bookings.Pricing;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.GetQuote;

public class GetQuoteProductConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IRentalPricingService pricingService) : IConsumer<GetQuoteProductCommand>
{
    public async Task Consume(ConsumeContext<GetQuoteProductCommand> context)
    {
        var command = context.Message;
        var ct = context.CancellationToken;

        var product = await uowFactory.ExecuteAsync(
            uow => uow.Products.GetByIdAsync(command.Id, ct),
            ct);

        if (product is null)
        {
            throw new ObjectNotFoundException(
                $"Product with ID: '{command.Id}' not found.");
        }

        var response = pricingService.Calculate(
            product,
            command.StartAt,
            command.EndAt);

        await context.RespondAsync(response);
    }
}