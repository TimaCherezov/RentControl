using Main.Api.Exceptions;
using Main.Api.Features.Bookings;
using Main.Api.Features.ProductImages;
using Main.Api.Features.Products;
using Main.Api.Features.Products.Get;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Products;

public class GetProductConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetProductConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _factory.RunsWith<Product>(_uow);
    }

    [Fact]
    public async Task Consume_WhenProductNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetProductCommand(productId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenProductExists_RespondsWithProductDto()
    {
        var productId = Guid.NewGuid();
        var product = new ProductBuilder()
            .WithId(productId)
            .WithMarketValue(1000m)
            .Build();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetProductCommand(productId));
        ProductDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductDto>()))
            .Callback<ProductDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(productId, responded.Id);
        Assert.Equal(300m, responded.DepositAmount);
    }

    private GetProductConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }),
        Options.Create(new BookingOptions()));
}
