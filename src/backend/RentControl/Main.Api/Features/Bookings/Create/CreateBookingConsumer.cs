using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.IntegrationEvents;

namespace Main.Api.Features.Bookings.Create;

public class CreateBookingConsumer(IMainUnitOfWork uow, IPublishEndpoint publishEndpoint)
    : IConsumer<CreateBookingCommand>
{
    public async Task Consume(ConsumeContext<CreateBookingCommand> context)
    {
        var ct = context.CancellationToken;
        var booking = context.Message.Adapt<Booking>();

        var createdBooking = await uow.ExecuteInTransactionAsync(
            async () =>
            {
                _ =
                    await uow.ProductUnits.GetByIdAsync(booking.ProductUnitId, ct)
                    ?? throw new ObjectNotFoundException(
                        $"ProductUnit with ID: {booking.ProductUnitId} not found."
                    ); // pessimistic lock

                if (
                    await uow.Bookings.HasOverlappingBookingAsync(
                        booking.ProductUnitId,
                        booking.StartAt,
                        booking.EndAt,
                        ct
                    )
                )
                    throw new InvalidOperationException(
                        $"Product unit with ID: {booking.ProductUnitId} is already booked for the period {booking.StartAt:O} – {booking.EndAt:O}."
                    );

                uow.Bookings.Add(booking);
                await publishEndpoint.Publish(
                    new OrderCreated(booking.Id, booking.DepositAmount),
                    ct
                );
                return booking;
            },
            ct
        );

        await context.RespondAsync(createdBooking.Adapt<BookingDto>());
    }
}
