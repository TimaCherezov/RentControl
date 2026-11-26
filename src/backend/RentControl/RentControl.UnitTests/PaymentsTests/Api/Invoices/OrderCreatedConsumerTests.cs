using Mapster;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Payments.Api.Features.Invoices;
using Payments.Api.Mappers;
using Payments.Model.Models;
using Payments.Model.Repositories;
using Payments.Model.UnitOfWork;
using RentControl.Common.IntegrationEvents;

namespace RentControl.UnitTests.PaymentsTests.Api.Invoices;

public class OrderCreatedConsumerTests
{
    private static readonly Guid BookingId = Guid.NewGuid();
    private const decimal Amount = 130m;

    private readonly Mock<IPaymentsUnitOfWork> _uow = new();
    private readonly Mock<IInvoiceRepository> _invoices = new();
    private Invoice? _addedInvoice;
    private InvoiceCreated? _publishedEvent;

    static OrderCreatedConsumerTests()
    {
        OrderCreatedMapConfig.Configure(TypeAdapterConfig.GlobalSettings);
    }

    public OrderCreatedConsumerTests()
    {
        _uow.SetupGet(u => u.Invoices).Returns(_invoices.Object);
        _invoices
            .Setup(r => r.Add(It.IsAny<Invoice>()))
            .Callback<Invoice>(invoice => _addedInvoice = invoice);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Consume_WhenInvoiceAlreadyExists_SkipsWithoutAddingSavingOrPublishing()
    {
        _invoices
            .Setup(r => r.ExistsByBookingIdAsync(BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var consumer = CreateConsumer();
        var context = CreateContext();

        await consumer.Consume(context.Object);

        _invoices.Verify(r => r.Add(It.IsAny<Invoice>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        context.Verify(c => c.Publish(It.IsAny<InvoiceCreated>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Consume_WhenNewBooking_AddsPendingInvoiceWithMappedFields()
    {
        var consumer = CreateConsumer();
        var context = CreateContext();

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedInvoice);
        Assert.Equal(BookingId, _addedInvoice.BookingId);
        Assert.Equal(Amount, _addedInvoice.Amount);
        Assert.Equal(InvoiceStatus.Pending, _addedInvoice.Status);
    }

    [Fact]
    public async Task Consume_WhenNewBooking_PublishesInvoiceCreatedWithBookingId()
    {
        var consumer = CreateConsumer();
        var context = CreateContext();

        await consumer.Consume(context.Object);

        Assert.Equal(BookingId, _publishedEvent?.BookingId);
    }

    private OrderCreatedConsumer CreateConsumer() => new(_uow.Object, Mock.Of<ILogger<OrderCreatedConsumer>>());

    private Mock<ConsumeContext<OrderCreated>> CreateContext()
    {
        var context = TestConsumeContext.For(new OrderCreated(BookingId, Amount));
        context
            .Setup(c => c.Publish(It.IsAny<InvoiceCreated>(), It.IsAny<CancellationToken>()))
            .Callback<InvoiceCreated, CancellationToken>((message, _) => _publishedEvent = message)
            .Returns(Task.CompletedTask);
        return context;
    }
}
