using Main.Api.Features.Bookings;
using Main.Api.Features.ProductImages;
using Main.Api.Features.Products;
using Main.Api.Features.Products.GetPage;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class GetPageProductConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetPageProductConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _factory.RunsWith<PageList<Product>>(_uow);
    }

    [Fact]
    public async Task Consume_WhenPageExists_RespondsWithPagedProductDtos()
    {
        var products = new List<Product>
        {
            new ProductBuilder().Build(),
            new ProductBuilder().Build(),
        };
        _products
            .Setup(p => p.GetPageAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageList<Product>(products, totalCount: 5, pageNumber: 1, pageSize: 2));
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetPageProductCommand(1, 2));
        PagedResponse<ProductDto>? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<PagedResponse<ProductDto>>()))
            .Callback<PagedResponse<ProductDto>>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(2, responded.Items.Count);
        Assert.Equal(5, responded.TotalCount);
        Assert.Equal(3, responded.TotalPages);
    }

    private GetPageProductConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }),
        Options.Create(new BookingOptions()));
}
