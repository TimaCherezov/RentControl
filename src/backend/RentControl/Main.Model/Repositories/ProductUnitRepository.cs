using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductUnitRepository(MainDbContext dbContext) : IProductUnitRepository
{
    public void Add(ProductUnit productUnit)
        => dbContext.ProductUnits.Add(productUnit);

    public Task<ProductUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.ProductUnits.FirstOrDefaultAsync(pu => pu.Id == id, cancellationToken);

    public void Update(ProductUnit productUnit)
        => dbContext.ProductUnits.Update(productUnit);
}
