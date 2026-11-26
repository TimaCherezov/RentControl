using Main.Api.Exceptions;
using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Bookings.Pricing;
using Main.Api.Features.ProductImages;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.IntegrationEvents;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class CreateBookingConsumerTests
{
    private static readonly DateTimeOffset StartAt = new(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset EndAt = new(2026, 7, 2, 10, 0, 0, TimeSpan.Zero);

    private static readonly BookingQuote Quote =
        new(Guid.Empty, StartAt, EndAt, 86400, 100m, 30m, 130m, []);

    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IProductUnitRepository> _productUnits = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();
    private readonly Mock<IRentalPricingService> _pricing = new();

    private Booking? _addedBooking;
    private OrderCreated? _publishedEvent;

    public CreateBookingConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _uow.SetupGet(u => u.ProductUnits).Returns(_productUnits.Object);
        _uow.SetupGet(u => u.Bookings).Returns(_bookings.Object);
        _uow
            .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<Booking>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<Booking>>, CancellationToken>((operation, _) => operation());
        _pricing
            .Setup(s => s.Calculate(It.IsAny<Product>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .Returns(Quote);
        _bookings
            .Setup(b => b.Add(It.IsAny<Booking>()))
            .Callback<Booking>(booking => _addedBooking = booking);
        _bookings
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, CancellationToken __) => _addedBooking);
        _publishEndpoint
            .Setup(p => p.Publish(It.IsAny<OrderCreated>(), It.IsAny<CancellationToken>()))
            .Callback<OrderCreated, CancellationToken>((message, _) => _publishedEvent = message)
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Consume_WhenProductNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenNoSuitableUnit_ThrowsUnprocessableEntityException()
    {
        var productId = Guid.NewGuid();
        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(new ProductBuilder().Build());
        _productUnits
            .Setup(p => p.GetBestFitUnitIdAsync(productId, StartAt, EndAt, It.IsAny<TimeSpan>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenLockedUnitNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(new ProductBuilder().Build());
        _productUnits
            .Setup(p => p.GetBestFitUnitIdAsync(productId, StartAt, EndAt, It.IsAny<TimeSpan>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unitId);
        _productUnits
            .Setup(p => p.GetByIdAsync(unitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductUnit?)null);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitBelongsToAnotherProduct_ThrowsUnprocessableEntityException()
    {
        var productId = Guid.NewGuid();
        var unit = new ProductUnitBuilder().WithProductId(Guid.NewGuid()).Build();
        ArrangeProductAndUnit(productId, unit);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitIsNotRentable_ThrowsUnprocessableEntityException()
    {
        var productId = Guid.NewGuid();
        var unit = new ProductUnitBuilder()
            .WithProductId(productId)
            .WithState(StateEnum.Broken)
            .Build();
        ArrangeProductAndUnit(productId, unit);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitInOtherLocation_ThrowsUnprocessableEntityException()
    {
        var productId = Guid.NewGuid();
        var requestedLocationId = Guid.NewGuid();
        var unit = new ProductUnitBuilder()
            .WithProductId(productId)
            .WithLocationId(Guid.NewGuid())
            .Build();
        ArrangeProductAndUnit(productId, unit);
        var consumer = CreateConsumer();
        var context = CreateContext(productId, locationId: requestedLocationId);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitAlreadyBooked_ThrowsConflictException()
    {
        var productId = Guid.NewGuid();
        var unit = new ProductUnitBuilder().WithProductId(productId).Build();
        ArrangeProductAndUnit(productId, unit);
        _bookings
            .Setup(b => b.HasOverlappingBookingAsync(unit.Id, StartAt, EndAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await Assert.ThrowsAsync<ConflictException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenValid_AddsPendingBookingWithQuoteAmounts()
    {
        var productId = Guid.NewGuid();
        ArrangeHappyPath(productId);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedBooking);
        Assert.Equal(BookingStatusEnum.Pending, _addedBooking.BookingStatus);
        Assert.Equal(100m, _addedBooking.TotalRentalCost);
        Assert.Equal(30m, _addedBooking.DepositAmount);
    }

    [Fact]
    public async Task Consume_WhenValid_PublishesOrderCreatedWithQuoteTotalAmount()
    {
        var productId = Guid.NewGuid();
        ArrangeHappyPath(productId);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);

        await consumer.Consume(context.Object);

        Assert.Equal(130m, _publishedEvent?.Amount);
    }

    [Fact]
    public async Task Consume_WhenValid_RespondsWithCreatedBookingDto()
    {
        var productId = Guid.NewGuid();
        ArrangeHappyPath(productId);
        var consumer = CreateConsumer();
        var context = CreateContext(productId);
        BookingDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<BookingDto>()))
            .Callback<BookingDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(_addedBooking?.Id, responded.Id);
        Assert.Equal(BookingStatusEnum.Pending, responded.BookingStatus);
    }

    private CreateBookingConsumer CreateConsumer() => new(
        _uow.Object,
        _publishEndpoint.Object,
        _pricing.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }),
        Options.Create(new BookingOptions()));

    private static Mock<ConsumeContext<CreateBookingCommand>> CreateContext(
        Guid productId,
        Guid? locationId = null,
        Guid? userId = null) =>
        TestConsumeContext.For(new CreateBookingCommand(
            productId,
            locationId,
            StartAt,
            EndAt,
            userId ?? Guid.NewGuid()));

    private void ArrangeProductAndUnit(Guid productId, ProductUnit unit)
    {
        _products
            .Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductBuilder().WithId(productId).Build());
        _productUnits
            .Setup(p => p.GetBestFitUnitIdAsync(productId, StartAt, EndAt, It.IsAny<TimeSpan>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(unit.Id);
        _productUnits
            .Setup(p => p.GetByIdAsync(unit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unit);
        _bookings
            .Setup(b => b.HasOverlappingBookingAsync(unit.Id, StartAt, EndAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    private void ArrangeHappyPath(Guid productId)
    {
        var unit = new ProductUnitBuilder().WithProductId(productId).Build();
        ArrangeProductAndUnit(productId, unit);
    }
}
