using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class ProductImageBuilder
{
    private static readonly DateTimeOffset DefaultCreatedAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private Guid _id = Guid.NewGuid();
    private Guid _productId = Guid.NewGuid();
    private string _storageKey = "images/photo.jpg";
    private bool _isMain;
    private DateTimeOffset _createdAt = DefaultCreatedAt;
    private string? _altText;

    public ProductImageBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ProductImageBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public ProductImageBuilder WithStorageKey(string storageKey)
    {
        _storageKey = storageKey;
        return this;
    }

    public ProductImageBuilder AsMain()
    {
        _isMain = true;
        return this;
    }

    public ProductImageBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ProductImage Build() => new()
    {
        Id = _id,
        ProductId = _productId,
        StorageKey = _storageKey,
        IsMain = _isMain,
        CreatedAt = _createdAt,
        AltText = _altText,
    };
}
