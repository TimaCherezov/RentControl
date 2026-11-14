using Main.Api.Exceptions;
using Main.Api.Features.ProductImages;
using Main.Api.Mappers;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Get;

public class GetBookingConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IOptions<ImageStorageOptions> imageStorageOptions
) : IConsumer<GetBookingCommand>
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

        var response = booking.ToDto(imageStorageOptions.Value);

        await context.RespondAsync(response);
    }
}
