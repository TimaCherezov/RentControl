using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.Bookings.Pricing;
using Main.Api.Features.Products.GetQuote;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Products;

public class GetQuoteProductConsumerTests
{
    private static readonly DateTimeOffset StartAt = new(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset EndAt = new(2026, 7, 2, 10, 0, 0, TimeSpan.Zero);

    private static readonly BookingQuote Quote =
        new(Guid.Empty, StartAt, EndAt, 86400, 100m, 30m, 130m, []);

    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<IRentalPricingService> _pricing = new();

    public GetQuoteProductConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _factory.RunsWith<Product>(_uow);
        _pricing
            .Setup(s => s.Calculate(It.IsAny<Product>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .Returns(Quote);
    }

    [Fact]
    public async Task Consume_WhenProductNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetQuoteProductCommand(productId, StartAt, EndAt));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenProductExists_RespondsWithQuote()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductBuilder().WithId(productId).Build());
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetQuoteProductCommand(productId, StartAt, EndAt));
        BookingQuote? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<BookingQuote>()))
            .Callback<BookingQuote>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.Equal(130m, responded?.TotalAmount);
    }

    private GetQuoteProductConsumer CreateConsumer() => new(_factory.Object, _pricing.Object);
}
