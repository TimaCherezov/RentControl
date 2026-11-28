using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Payments.Api.Features.Invoices.Pay;
using Payments.Api.Exceptions;
using RentControl.ServiceDefaults.Exceptions;
using Payments.Model.Models;
using Payments.Model.Repositories;
using Payments.Model.UnitOfWork;

namespace RentControl.UnitTests.PaymentsTests.Api.Invoices.Pay;

public class PayInvoiceConsumerTests
{
    private static readonly Guid BookingId = Guid.NewGuid();
    private static readonly Guid InvoiceId = Guid.NewGuid();

    private readonly Mock<IPaymentsUnitOfWork> _uow = new();
    private readonly Mock<IInvoiceRepository> _invoices = new();

    public PayInvoiceConsumerTests()
    {
        _uow.SetupGet(u => u.Invoices).Returns(_invoices.Object);
    }

    [Fact]
    public async Task Consume_WhenInvoiceNotFoundByBooking_ThrowsObjectNotFoundException()
    {
        _invoices
            .Setup(r => r.GetByBookingIdAsync(BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payments.Model.Models.Invoice?)null);
        var consumer = CreateConsumer();
        var context = CreateContext();

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenInvoiceNotPending_ThrowsPaymentAlreadyPaidException()
    {
        _invoices
            .Setup(r => r.GetByBookingIdAsync(BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvoiceBuilder().WithId(InvoiceId).WithStatus(InvoiceStatus.Paid).Build());
        var consumer = CreateConsumer();
        var context = CreateContext();

        await Assert.ThrowsAsync<PaymentAlreadyPaidException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenInvoicePending_RespondsWithFakePayUrlForInvoice()
    {
        _invoices
            .Setup(r => r.GetByBookingIdAsync(BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvoiceBuilder().WithId(InvoiceId).Build());
        var consumer = CreateConsumer();
        var context = CreateContext();
        PayInvoiceResponse? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<PayInvoiceResponse>()))
            .Callback<PayInvoiceResponse>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.Equal($"/fake-pay/{InvoiceId}", responded?.PaymentUrl);
    }

    private PayInvoiceConsumer CreateConsumer() => new(_uow.Object);

    private static Mock<ConsumeContext<PayInvoiceCommand>> CreateContext() =>
        TestConsumeContext.For(new PayInvoiceCommand(BookingId));
}
