using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Create;

public class CreateBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<CreateBookingCommand>
{
    public async Task Consume(ConsumeContext<CreateBookingCommand> context)
    {
        var command = context.Message;

        var booking = await uowFactory.ExecuteAsync(
            uow =>
            {
                var bookingModel = command.Adapt<Booking>();
                uow.Bookings.Add(bookingModel);
                return Task.FromResult(bookingModel);
            },
            context.CancellationToken
        );

        var response = booking.Adapt<BookingDto>();

        await context.RespondAsync(response);
    }
}
