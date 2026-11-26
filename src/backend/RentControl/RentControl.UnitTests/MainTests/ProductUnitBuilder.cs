using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class ProductUnitBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _productId = Guid.NewGuid();
    private Guid _locationId = Guid.NewGuid();
    private StateEnum _state = StateEnum.New;
    private string _serialNumber = "SN-0001";

    public ProductUnitBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ProductUnitBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public ProductUnitBuilder WithLocationId(Guid locationId)
    {
        _locationId = locationId;
        return this;
    }

    public ProductUnitBuilder WithState(StateEnum state)
    {
        _state = state;
        return this;
    }

    public ProductUnitBuilder WithSerialNumber(string serialNumber)
    {
        _serialNumber = serialNumber;
        return this;
    }

    public ProductUnit Build() => new()
    {
        Id = _id,
        ProductId = _productId,
        LocationId = _locationId,
        State = _state,
        SerialNumber = _serialNumber,
    };
}
