using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.ProductUnits;
using Main.Api.Features.ProductUnits.Create;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.ProductUnits;

public class CreateProductUnitConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductUnitRepository> _productUnits = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    private ProductUnit? _addedUnit;

    public CreateProductUnitConsumerTests()
    {
        _uow.SetupGet(u => u.ProductUnits).Returns(_productUnits.Object);
        _factory.RunsWith<ProductUnit>(_uow);
        _productUnits
            .Setup(p => p.Add(It.IsAny<ProductUnit>()))
            .Callback<ProductUnit>(unit => _addedUnit = unit);
    }

    [Fact]
    public async Task Consume_WhenSerialNumberAlreadyExists_ThrowsConflictException()
    {
        var productId = Guid.NewGuid();
        _productUnits
            .Setup(p => p.GetByProductAndSerialNumberAsync(productId, "SN-0001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductUnitBuilder().WithSerialNumber("SN-0001").Build());
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand(productId));

        await Assert.ThrowsAsync<ConflictException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenSerialNumberIsUnique_AddsUnitAndRespondsWithDto()
    {
        var productId = Guid.NewGuid();
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand(productId));
        ProductUnitDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductUnitDto>()))
            .Callback<ProductUnitDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedUnit);
        Assert.Equal("SN-0001", _addedUnit.SerialNumber);
        Assert.Equal("SN-0001", responded?.SerialNumber);
    }

    private static CreateProductUnitCommand CreateCommand(Guid productId) => new(
        productId,
        State: StateEnum.New,
        SerialNumber: "SN-0001",
        PurchaseDate: new DateOnly(2026, 1, 15),
        PurchasePrice: 900m,
        LocationId: Guid.NewGuid());

    private CreateProductUnitConsumer CreateConsumer() => new(_factory.Object);
}
