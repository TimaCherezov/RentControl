using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.ProductUnits.Delete;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.ProductUnits;

public class DeleteProductUnitConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductUnitRepository> _productUnits = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public DeleteProductUnitConsumerTests()
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
        var context = TestConsumeContext.For(new DeleteProductUnitCommand(unitId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitExists_RemovesUnit()
    {
        var unit = new ProductUnitBuilder().Build();
        _productUnits
            .Setup(p => p.GetByIdAsync(unit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unit);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductUnitCommand(unit.Id));

        await consumer.Consume(context.Object);

        _productUnits.Verify(p => p.Remove(It.Is<ProductUnit>(removed => removed.Id == unit.Id)), Times.Once);
    }

    private DeleteProductUnitConsumer CreateConsumer() => new(_factory.Object);
}
