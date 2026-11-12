using Main.Api.Exceptions;
using Main.Api.Mappers;
using Main.Api.Storage;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.IntegrationEvents;

namespace Main.Api.Features.Bookings.Create;

public class CreateBookingConsumer(
    IMainUnitOfWork uow,
    IPublishEndpoint publishEndpoint,
    ImageStorageOptions opt
) : IConsumer<CreateBookingCommand>
{
    public async Task Consume(ConsumeContext<CreateBookingCommand> context)
    {
        var ct = context.CancellationToken;
        var booking = context.Message.Adapt<Booking>();

        await uow.ExecuteInTransactionAsync(
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

                booking.Id = Guid.NewGuid(); // for publish (default empty)
                booking.BookingStatus = BookingStatusEnum.Pending;

                uow.Bookings.Add(booking);
                await publishEndpoint.Publish(
                    new OrderCreated(booking.Id, booking.DepositAmount),
                    ct
                );
                return Task.CompletedTask;
            },
            ct
        );

        var createdBooking =
            await uow.Bookings.GetByIdAsync(booking.Id, ct)
            ?? throw new ObjectNotFoundException($"Booking with ID: {booking.Id} not found.");

        var response = createdBooking.ToDto(opt);

        await context.RespondAsync(response);
    }
}
