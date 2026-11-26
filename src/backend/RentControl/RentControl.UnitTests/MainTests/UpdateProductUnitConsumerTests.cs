using Main.Api.Exceptions;
using Main.Api.Features.ProductUnits.Update;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class UpdateProductUnitConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductUnitRepository> _productUnits = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public UpdateProductUnitConsumerTests()
    {
        _uow.SetupGet(u => u.ProductUnits).Returns(_productUnits.Object);
        _factory.RunsWith(_uow);
    }

    [Fact]
    public async Task Consume_WhenUnitNotFound_ThrowsObjectNotFoundException()
    {
        var unitId = Guid.NewGuid();
        _productUnits
            .Setup(p => p.GetByIdAsync(unitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductUnit?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand(unitId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitExists_AppliesCommandToUnit()
    {
        var unit = new ProductUnitBuilder().WithState(StateEnum.New).Build();
        _productUnits
            .Setup(p => p.GetByIdAsync(unit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unit);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand(unit.Id));

        await consumer.Consume(context.Object);

        Assert.Equal(StateEnum.Broken, unit.State);
    }

    private static UpdateProductUnitCommand CreateCommand(Guid unitId) => new(
        unitId,
        Guid.NewGuid(),
        State: StateEnum.Broken,
        SerialNumber: "SN-0002",
        PurchaseDate: new DateOnly(2026, 2, 1),
        PurchasePrice: 800m,
        LocationId: Guid.NewGuid());

    private UpdateProductUnitConsumer CreateConsumer() => new(_factory.Object);
}
