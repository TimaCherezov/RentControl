using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class LocationBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Central Point";
    private string _address = "Main Street 1";

    public LocationBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public LocationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public LocationBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public Location Build() => new()
    {
        Id = _id,
        Name = _name,
        Address = _address,
    };
}
