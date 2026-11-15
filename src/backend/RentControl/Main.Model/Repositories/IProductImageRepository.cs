using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IProductImageRepository
{
    Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(ProductImage productImage);
    void Remove(ProductImage productImage);
    Task<ProductImage?> GetMainByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    );
    Task<List<ProductImage>> GetAllByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    );
    Task<Guid?> GetNextMainCandidateIdByProductIdAsync(
        Guid productId,
        Guid excludeId,
        CancellationToken cancellationToken = default
    );
    Task SetMainAsync(
        Guid productId,
        Guid mainImageId,
        CancellationToken cancellationToken = default
    );
}
