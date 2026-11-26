using Main.Api.Exceptions;
using Main.Api.Features.ProductImages.Delete;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class DeleteProductImageConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductImageRepository> _productImages = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public DeleteProductImageConsumerTests()
    {
        _uow.SetupGet(u => u.ProductImages).Returns(_productImages.Object);
        _factory.RunsWith(_uow);
    }

    [Fact]
    public async Task Consume_WhenImageNotFound_ThrowsObjectNotFoundException()
    {
        var imageId = Guid.NewGuid();
        _productImages
            .Setup(p => p.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductImage?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductImageCommand(imageId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenDeletingNonMainImage_DoesNotChangeMain()
    {
        var image = new ProductImageBuilder().Build();
        _productImages
            .Setup(p => p.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductImageCommand(image.Id));

        await consumer.Consume(context.Object);

        _productImages.Verify(
            p => p.SetMainAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WhenDeletingMainImageWithCandidate_PromotesNext()
    {
        var image = new ProductImageBuilder().AsMain().Build();
        var candidateId = Guid.NewGuid();
        _productImages
            .Setup(p => p.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        _productImages
            .Setup(p => p.GetNextMainCandidateIdByProductIdAsync(image.ProductId, image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(candidateId);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductImageCommand(image.Id));

        await consumer.Consume(context.Object);

        _productImages.Verify(
            p => p.SetMainAsync(image.ProductId, candidateId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenDeletingMainImageWithoutCandidate_SucceedsWithoutPromotion()
    {
        var image = new ProductImageBuilder().AsMain().Build();
        _productImages
            .Setup(p => p.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        _productImages
            .Setup(p => p.GetNextMainCandidateIdByProductIdAsync(image.ProductId, image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteProductImageCommand(image.Id));

        await consumer.Consume(context.Object);

        _productImages.Verify(
            p => p.SetMainAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private DeleteProductImageConsumer CreateConsumer() => new(_factory.Object);
}
