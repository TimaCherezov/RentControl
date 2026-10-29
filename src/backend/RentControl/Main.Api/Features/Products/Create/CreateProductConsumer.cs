using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using MassTransit.Mediator;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Create;

public class CreateProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<CreateProductCommand>
{
    public async Task Consume(ConsumeContext<CreateProductCommand> context)
    {
        var command = context.Message;

        var product = await uowFactory.ExecuteAsync(async uow =>
        {
            var product = new Product
            {
                Name = command.Name,
                MarketValue = command.MarketValue,
                DepositOverride = command.DepositOverride,
                DailyRate = command.DailyRate,
                HourlyRate = command.HourlyRate,
                WeeklyRate = command.WeeklyRate,
                MonthlyRate = command.MonthlyRate,
                Description = command.Description
            };

            await uow.Products.AddAsync(product);

            return product;
        }, context.CancellationToken);

        var response = new ProductDto(
            product.Id,
            product.Name,
            product.MarketValue,
            product.DepositOverride,
            product.DailyRate,
            product.HourlyRate,
            product.WeeklyRate,
            product.MonthlyRate,
            product.Description);

        await context.RespondAsync(response);
    }
}
