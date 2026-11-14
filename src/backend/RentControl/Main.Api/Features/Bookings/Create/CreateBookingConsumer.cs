using Main.Api.Exceptions;
using Main.Api.Features.ProductImages;
using Main.Api.Mappers;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using RentControl.Common.IntegrationEvents;

namespace Main.Api.Features.Bookings.Create;

public class CreateBookingConsumer(
    IMainUnitOfWork uow,
    IPublishEndpoint publishEndpoint,
    IOptions<ImageStorageOptions> imageStorageOptions,
    IOptions<BookingOptions> bookingOptions
) : IConsumer<CreateBookingCommand>
{
    public async Task Consume(ConsumeContext<CreateBookingCommand> context)
    {
        var ct = context.CancellationToken;
        var command = context.Message;

        var booking = await uow.ExecuteInTransactionAsync(
            async () =>
            {
                var product =
                    await uow.Products.GetByIdAsync(command.ProductId, ct)
                    ?? throw new ObjectNotFoundException(
                        $"Product with ID: {command.ProductId} not found."
                    );

                var productUnitId =
                    await uow.ProductUnits.GetBestFitUnitIdAsync(
                        command.ProductId,
                        command.StartAt,
                        command.EndAt,
                        bookingOptions.Value.MaxBookingDuration,
                        command.LocationId,
                        ct
                    )
                    ?? throw new InvalidOperationException(
                        $"Could not find a suitable product unit. ProductId: {command.ProductId}, StartAt: {command.StartAt:O}, EndAt: {command.EndAt:O}"
                    );

                var productUnit =
                    await uow.ProductUnits.GetByIdAsync(productUnitId, ct)
                    ?? throw new ObjectNotFoundException(
                        $"ProductUnit with ID: {productUnitId} not found."
                    ); // pessimistic lock

                if (productUnit.ProductId != command.ProductId)
                    throw new InvalidOperationException(
                        $"Product unit with ID: {productUnit.Id} does not belong to product with ID: {command.ProductId}."
                    );

                if (!StatesExtensions.IsRentable(productUnit.State))
                    throw new InvalidOperationException(
                        $"Product unit with ID: {productUnit.Id} is not rentable in state '{productUnit.State}'."
                    );

                if (command.LocationId != null && productUnit.LocationId != command.LocationId)
                    throw new InvalidOperationException(
                        $"Product unit with ID: {productUnit.Id} belongs to location {productUnit.LocationId}, not the requested location {command.LocationId}."
                    );

                if (
                    await uow.Bookings.HasOverlappingBookingAsync(
                        productUnitId,
                        command.StartAt,
                        command.EndAt,
                        ct
                    )
                )
                    throw new InvalidOperationException(
                        $"Product unit with ID: {productUnitId} is already booked for the period {command.StartAt:O} – {command.EndAt:O}."
                    );

                var bookingModel = new Booking
                {
                    Id = Guid.NewGuid(),
                    ProductUnitId = productUnit.Id,
                    UserId = command.UserId,
                    LocationId = productUnit.LocationId,
                    StartAt = command.StartAt,
                    EndAt = command.EndAt,
                    DepositAmount = product.CalculateDeposit(bookingOptions.Value.DepositRate),
                    BookingStatus = BookingStatusEnum.Pending,
                    StatusChangedAt = DateTimeOffset.UtcNow,
                };

                uow.Bookings.Add(bookingModel);

                await publishEndpoint.Publish(
                    new OrderCreated(bookingModel.Id, bookingModel.DepositAmount), //TODO: + аренда
                    ct
                );
                return bookingModel;
            },
            ct
        );

        var createdBooking =
            await uow.Bookings.GetByIdAsync(booking.Id, ct)
            ?? throw new ObjectNotFoundException($"Booking with ID: {booking.Id} not found.");

        var response = createdBooking.ToDto(imageStorageOptions.Value);

        await context.RespondAsync(response);
    }
}
