using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductRepository(MainDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public void Add(Product product)
        => dbContext.Products.Add(product);

    public void Update(Product product)
        => dbContext.Products.Update(product);
}
