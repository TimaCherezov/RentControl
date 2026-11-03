using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Update;

public class UpdateBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<UpdateBookingCommand>
{
    public async Task Consume(ConsumeContext<UpdateBookingCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(
            async uow =>
            {
                var bookingModel =
                    await uow.Bookings.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Booking with ID:'{command.Id}' not found"
                    );
                command.Adapt(bookingModel);
            },
            context.CancellationToken
        );
    }
}
