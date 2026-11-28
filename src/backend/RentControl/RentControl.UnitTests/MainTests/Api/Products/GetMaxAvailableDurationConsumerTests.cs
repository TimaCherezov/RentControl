using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.Bookings;
using Main.Api.Features.Products;
using Main.Api.Features.Products.GetMaxAvailableDuration;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Products;

public class GetMaxAvailableDurationConsumerTests
{
    private static readonly DateTimeOffset StartAt = new(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan MaxDuration = TimeSpan.FromDays(5);

    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IProductUnitRepository> _productUnits = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetMaxAvailableDurationConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _uow.SetupGet(u => u.ProductUnits).Returns(_productUnits.Object);
        _factory.RunsWith<TimeSpan>(_uow);
        _productUnits
            .Setup(p => p.GetMaxAvailableDurationAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<TimeSpan>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MaxDuration);
    }

    [Fact]
    public async Task Consume_WhenProductNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetMaxAvailableDurationCommand(productId, StartAt, null));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenProductExists_RespondsWithMaxAvailableDuration()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductBuilder().WithId(productId).Build());
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetMaxAvailableDurationCommand(productId, StartAt, null));
        ProductMaxAvailableDuration? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductMaxAvailableDuration>()))
            .Callback<ProductMaxAvailableDuration>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.Equal(TimeSpan.FromDays(5), responded?.MaxDuration);
        Assert.Equal(432000, responded?.TotalSeconds);
    }

    private GetMaxAvailableDurationConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new BookingOptions()));
}
