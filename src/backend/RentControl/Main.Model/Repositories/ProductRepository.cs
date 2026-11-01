using Main.Model.Models;
using Main.Model.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductRepository(MainDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public void Add(Product product, CancellationToken cancellationToken = default)
        => dbContext.Products.Add(product);
}
