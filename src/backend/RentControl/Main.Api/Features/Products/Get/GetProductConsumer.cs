using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;
using Main.Api.Mappers;
using Main.Api.Features.ProductImages;
using Microsoft.Extensions.Options;
using Main.Api.Features.Bookings;

namespace Main.Api.Features.Products.Get;

public class GetProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IOptions<ImageStorageOptions> imageStorageOptions, IOptions<BookingOptions> bookingOptions)
    : IConsumer<GetProductCommand>
{
    public async Task Consume(ConsumeContext<GetProductCommand> context)
    {
        var command = context.Message;

        var product = await uowFactory.ExecuteAsync(
            (async uow =>
            {
                var productModel =
                    await uow.Products.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Product with ID:'{command.Id}' not found"
                    );
                return productModel;
            }),
            context.CancellationToken
        );

        var response = product.ToDto(imageStorageOptions.Value, bookingOptions.Value);

        await context.RespondAsync(response);
    }
}
