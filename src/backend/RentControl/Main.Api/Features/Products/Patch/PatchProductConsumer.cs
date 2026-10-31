using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Patch;

public class PatchProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<PatchProductCommand>
{
    public async Task Consume(ConsumeContext<PatchProductCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var product = await uow.Products.GetByIdAsync(command.Id, context.CancellationToken);
            if (product is null)
                throw new NotFoundException($"Product with ID:'{command.Id}' not found");

            AddChangesToModel(product, command);

            uow.Products.Update(product);
        }, context.CancellationToken);
    }

    private static void AddChangesToModel(Product model, PatchProductCommand command)
    {
        if (command.Name is not null)
            model.Name = command.Name;
        if (command.MarketValue.HasValue)
            model.MarketValue = command.MarketValue.Value;
        if (command.DepositOverride.HasValue)
            model.DepositOverride = command.DepositOverride.Value;
        if (command.DailyRate.HasValue)
            model.DailyRate = command.DailyRate.Value;
        if (command.HourlyRate.HasValue)
            model.HourlyRate = command.HourlyRate.Value;
        if (command.WeeklyRate.HasValue)
            model.WeeklyRate = command.WeeklyRate;
        if (command.MonthlyRate.HasValue)
            model.MonthlyRate = command.MonthlyRate;
        if (command.Description is not null)
            model.Description = command.Description;
    }
}
