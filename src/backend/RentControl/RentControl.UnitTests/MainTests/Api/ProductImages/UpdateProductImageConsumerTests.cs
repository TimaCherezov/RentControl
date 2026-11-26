using Main.Api.Exceptions;
using Main.Api.Features.ProductImages.Update;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.ProductImages;

public class UpdateProductImageConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductImageRepository> _productImages = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public UpdateProductImageConsumerTests()
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
        var context = TestConsumeContext.For(new UpdateProductImageCommand(imageId, "images/new.jpg", IsMain: false, null));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenSettingMainOnNonMainImage_CallsSetMain()
    {
        var image = new ProductImageBuilder().Build();
        _productImages
            .Setup(p => p.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new UpdateProductImageCommand(image.Id, "images/new.jpg", IsMain: true, null));

        await consumer.Consume(context.Object);

        _productImages.Verify(
            p => p.SetMainAsync(image.ProductId, image.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenUnsettingMainWithoutCandidate_ThrowsConflictException()
    {
        var image = new ProductImageBuilder().AsMain().Build();
        _productImages
            .Setup(p => p.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        _productImages
            .Setup(p => p.GetNextMainCandidateIdByProductIdAsync(image.ProductId, image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new UpdateProductImageCommand(image.Id, "images/new.jpg", IsMain: false, null));

        await Assert.ThrowsAsync<ConflictException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUnsettingMainWithCandidate_PromotesCandidate()
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
        var context = TestConsumeContext.For(new UpdateProductImageCommand(image.Id, "images/new.jpg", IsMain: false, null));

        await consumer.Consume(context.Object);

        _productImages.Verify(
            p => p.SetMainAsync(image.ProductId, candidateId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenMainStaysMain_DoesNotCallSetMain()
    {
        var image = new ProductImageBuilder().AsMain().Build();
        _productImages
            .Setup(p => p.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new UpdateProductImageCommand(image.Id, "images/new.jpg", IsMain: true, null));

        await consumer.Consume(context.Object);

        _productImages.Verify(
            p => p.SetMainAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private UpdateProductImageConsumer CreateConsumer() => new(_factory.Object);
}
