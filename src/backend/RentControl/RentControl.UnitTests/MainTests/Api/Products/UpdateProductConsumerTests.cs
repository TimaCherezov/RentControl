using Main.Api.Exceptions;
using Main.Api.Features.Products.Update;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Products;

public class UpdateProductConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public UpdateProductConsumerTests()
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
        var context = TestConsumeContext.For(CreateCommand(productId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenProductExists_AppliesCommandToProduct()
    {
        var product = new ProductBuilder().WithName("Old name").Build();
        _products
            .Setup(p => p.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand(product.Id));

        await consumer.Consume(context.Object);

        Assert.Equal("New name", product.Name);
    }

    private static UpdateProductCommand CreateCommand(Guid productId) => new(
        productId,
        "New name",
        MarketValue: 2000m,
        DepositOverride: null,
        DailyRate: 700m,
        HourlyRate: 70m,
        WeeklyRate: null,
        MonthlyRate: null,
        Description: null);

    private UpdateProductConsumer CreateConsumer() => new(_factory.Object);
}
