using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageList<Product>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(Product product);
    void Update(Product product);
}
