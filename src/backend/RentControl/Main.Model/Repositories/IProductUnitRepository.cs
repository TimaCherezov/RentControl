using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IProductUnitRepository
{
    Task<ProductUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(ProductUnit product);
    void Update(ProductUnit product);
}
