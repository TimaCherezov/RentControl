using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductUnitRepository(MainDbContext dbcontext) : IProductUnitRepository
{
    public void Add(ProductUnit product)
        => dbcontext.ProductUnits.Add(product);

    public Task<ProductUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbcontext.ProductUnits.FirstOrDefaultAsync(pu => pu.Id == id, cancellationToken);

    public void Update(ProductUnit product)
        => dbcontext.ProductUnits.Update(product);
}
