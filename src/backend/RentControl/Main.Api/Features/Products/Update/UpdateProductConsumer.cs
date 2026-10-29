using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Update;

public class UpdateProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<UpdateProductCommand>
{
    public async Task Consume(ConsumeContext<UpdateProductCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var product = await uow.Products.GetByIdAsync(command.Id, context.CancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Product with id {command.Id} not found");
            }

            product.Name = command.Name;
            product.MarketValue = command.MarketValue;
            product.DepositOverride = command.DepositOverride;
            product.DailyRate = command.DailyRate;
            product.HourlyRate = command.HourlyRate;
            product.WeeklyRate = command.WeeklyRate;
            product.MonthlyRate = command.MonthlyRate;
            product.Description = command.Description;

            uow.Products.Update(product);
        }, context.CancellationToken);
    }
}
