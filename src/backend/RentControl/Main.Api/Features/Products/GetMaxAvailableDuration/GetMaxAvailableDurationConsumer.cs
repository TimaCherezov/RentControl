using Main.Api.Exceptions;
using Main.Api.Features.Bookings;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.GetMaxAvailableDuration;

public class GetMaxAvailableDurationConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IOptions<BookingOptions> bookingOptions
) : IConsumer<GetMaxAvailableDurationCommand>
{
    public async Task Consume(ConsumeContext<GetMaxAvailableDurationCommand> context)
    {
        var command = context.Message;
        var ct = context.CancellationToken;

        var maxDuration = await uowFactory.ExecuteAsync(
            async uow =>
            {
                _ =
                    await uow.Products.GetByIdAsync(command.Id, ct)
                    ?? throw new ObjectNotFoundException(
                        $"Product with ID: {command.Id} not found."
                    );

                return await uow.ProductUnits.GetMaxAvailableDurationAsync(
                    command.Id,
                    command.StartAt,
                    bookingOptions.Value.MaxBookingDuration,
                    command.LocationId,
                    ct
                );
            },
            ct
        );

        var response = new ProductMaxAvailableDuration(maxDuration, (long)maxDuration.TotalSeconds);

        await context.RespondAsync(response);
    }
}
