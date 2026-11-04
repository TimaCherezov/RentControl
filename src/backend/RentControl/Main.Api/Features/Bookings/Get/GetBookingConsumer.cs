using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Get;

public class GetBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetBookingCommand>
{
    public async Task Consume(ConsumeContext<GetBookingCommand> context)
    {
        var command = context.Message;

        var booking = await uowFactory.ExecuteAsync(
            async uow =>
            {
                var bookingModel =
                    await uow.Bookings.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Booking with ID:'{command.Id}' not found"
                    );
                return bookingModel;
            },
            context.CancellationToken
        );

        var response = booking.Adapt<BookingDto>();

        await context.RespondAsync(response);
    }
}
