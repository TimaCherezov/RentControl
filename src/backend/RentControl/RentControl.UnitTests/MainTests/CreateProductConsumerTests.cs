using Main.Api.Features.Bookings;
using Main.Api.Features.ProductImages;
using Main.Api.Features.Products;
using Main.Api.Features.Products.Create;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class CreateProductConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    private Product? _addedProduct;

    public CreateProductConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _factory.RunsWith<Product>(_uow);
        _products
            .Setup(p => p.Add(It.IsAny<Product>()))
            .Callback<Product>(product => _addedProduct = product);
    }

    [Fact]
    public async Task Consume_WhenValid_AddsProduct()
    {
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand());

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedProduct);
        Assert.Equal("Excavator", _addedProduct.Name);
    }

    [Fact]
    public async Task Consume_WhenValid_RespondsWithProductDtoIncludingComputedDeposit()
    {
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand());
        ProductDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductDto>()))
            .Callback<ProductDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal("Excavator", responded.Name);
        Assert.Equal(300m, responded.DepositAmount);
    }

    private static CreateProductCommand CreateCommand() => new(
        "Excavator",
        MarketValue: 1000m,
        DepositOverride: null,
        DailyRate: 500m,
        HourlyRate: 50m,
        WeeklyRate: null,
        MonthlyRate: null,
        Description: null);

    private CreateProductConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }),
        Options.Create(new BookingOptions()));
}
