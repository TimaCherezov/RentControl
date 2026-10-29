using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Patch;

public class PatchProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<PatchProductCommand>
{
    public async Task Consume(ConsumeContext<PatchProductCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var product = await uow.Products.GetByIdAsync(command.Id, context.CancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Product with id {command.Id} not found");
            }

            if (command.Name is not null)
                product.Name = command.Name;
            if (command.MarketValue.HasValue)
                product.MarketValue = command.MarketValue.Value;
            if (command.DepositOverride.HasValue)
                product.DepositOverride = command.DepositOverride.Value;
            if (command.DailyRate.HasValue)
                product.DailyRate = command.DailyRate.Value;
            if (command.HourlyRate.HasValue)
                product.HourlyRate = command.HourlyRate.Value;
            if (command.WeeklyRate.HasValue)
                product.WeeklyRate = command.WeeklyRate;
            if (command.MonthlyRate.HasValue)
                product.MonthlyRate = command.MonthlyRate;
            if (command.Description is not null)
                product.Description = command.Description;

            uow.Products.Update(product);
        }, context.CancellationToken);
    }
}
