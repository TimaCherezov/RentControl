using Main.Api.Exceptions;
using Main.Api.Features.ProductUnits;
using Main.Api.Features.ProductUnits.Get;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.ProductUnits;

public class GetProductUnitConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductUnitRepository> _productUnits = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetProductUnitConsumerTests()
    {
        _uow.SetupGet(u => u.ProductUnits).Returns(_productUnits.Object);
        _factory.RunsWith<ProductUnit>(_uow);
    }

    [Fact]
    public async Task Consume_WhenUnitNotFound_ThrowsObjectNotFoundException()
    {
        var unitId = Guid.NewGuid();
        _productUnits
            .Setup(p => p.GetByIdAsync(unitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductUnit?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetProductUnitCommand(unitId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnitExists_RespondsWithProductUnitDto()
    {
        var unit = new ProductUnitBuilder().WithSerialNumber("SN-0001").Build();
        _productUnits
            .Setup(p => p.GetByIdAsync(unit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unit);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetProductUnitCommand(unit.Id));
        ProductUnitDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductUnitDto>()))
            .Callback<ProductUnitDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(unit.Id, responded.Id);
        Assert.Equal("SN-0001", responded.SerialNumber);
    }

    private GetProductUnitConsumer CreateConsumer() => new(_factory.Object);
}
