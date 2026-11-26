using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class BookingBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();
    private Guid _productUnitId = Guid.NewGuid();
    private Guid _locationId = Guid.NewGuid();
    private BookingStatusEnum _bookingStatus = BookingStatusEnum.Pending;
    private DateTimeOffset? _actualReturnAt;
    private bool _isInvoiceCreated;

    public BookingBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BookingBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public BookingBuilder WithProductUnitId(Guid productUnitId)
    {
        _productUnitId = productUnitId;
        return this;
    }

    public BookingBuilder WithLocationId(Guid locationId)
    {
        _locationId = locationId;
        return this;
    }

    public BookingBuilder WithStatus(BookingStatusEnum bookingStatus)
    {
        _bookingStatus = bookingStatus;
        return this;
    }

    public BookingBuilder WithInvoiceCreated()
    {
        _isInvoiceCreated = true;
        return this;
    }

    public BookingBuilder WithActualReturnAt(DateTimeOffset actualReturnAt)
    {
        _actualReturnAt = actualReturnAt;
        return this;
    }

    public Booking Build() => new()
    {
        Id = _id,
        UserId = _userId,
        ProductUnitId = _productUnitId,
        LocationId = _locationId,
        BookingStatus = _bookingStatus,
        ActualReturnAt = _actualReturnAt,
        IsInvoiceCreated = _isInvoiceCreated,
    };
}
