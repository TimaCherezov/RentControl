using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.Products.Delete;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Products;

public class DeleteProductConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public DeleteProductConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _factory.RunsWith(_uow);
    }

    [Fact]
    public async Task Consume_WhenProductNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductCommand(productId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenProductExists_RemovesProduct()
    {
        var product = new ProductBuilder().Build();
        _products
            .Setup(p => p.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductCommand(product.Id));

        await consumer.Consume(context.Object);

        _products.Verify(p => p.Remove(It.Is<Product>(removed => removed.Id == product.Id)), Times.Once);
    }

    private DeleteProductConsumer CreateConsumer() => new(_factory.Object);
}
