using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Product product);
    void Update(Product product);
}
