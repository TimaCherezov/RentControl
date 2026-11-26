using Payments.Model.Models;

namespace RentControl.UnitTests.PaymentsTests;

internal class InvoiceBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _bookingId = Guid.NewGuid();
    private decimal _amount = 130m;
    private InvoiceStatus _status = InvoiceStatus.Pending;

    public InvoiceBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public InvoiceBuilder WithBookingId(Guid bookingId)
    {
        _bookingId = bookingId;
        return this;
    }

    public InvoiceBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public InvoiceBuilder WithStatus(InvoiceStatus status)
    {
        _status = status;
        return this;
    }

    public Invoice Build() => new()
    {
        Id = _id,
        BookingId = _bookingId,
        Amount = _amount,
        Status = _status,
    };
}
