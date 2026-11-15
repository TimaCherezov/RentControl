using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;
using Main.Api.Features.ProductImages;
using Main.Api.Mappers;
using Microsoft.Extensions.Options;
using Main.Api.Features.Bookings;

namespace Main.Api.Features.Products.GetPage;

public class GetPageProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IOptions<ImageStorageOptions> imageStorageOptions, IOptions<BookingOptions> bookingOptions)
    : IConsumer<GetPageProductCommand>
{
    public async Task Consume(ConsumeContext<GetPageProductCommand> context)
    {
        var command = context.Message;

        var productPageList = await uowFactory.ExecuteAsync(async uow =>
        {
            var pageList = await uow.Products.GetPageAsync(command.PageNumber, command.PageSize);
            return pageList;
        });

        var items = productPageList.ToDto(imageStorageOptions.Value, bookingOptions.Value);
        var response = new PagedResponse<ProductDto>(
            items,
            productPageList.TotalCount,
            productPageList.PageNumber,
            productPageList.PageSize
        );

        await context.RespondAsync(response);
    }
}
