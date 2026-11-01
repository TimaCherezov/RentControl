using Main.Model.Models;

namespace Main.Model.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Product product, CancellationToken cancellationToken = default);
}
