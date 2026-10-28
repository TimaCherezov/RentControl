using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IProductImageRepository
{
    Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(ProductImage productImage);
    void Update(ProductImage productImage);
    Task<ProductImage?> GetMainForProductByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    );
    Task<List<ProductImage>> GetAllForProductByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    );
}
