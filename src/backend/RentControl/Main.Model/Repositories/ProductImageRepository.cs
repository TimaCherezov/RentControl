using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductImageRepository(MainDbContext dbContext) : IProductImageRepository
{
    public void Add(ProductImage productImage) => dbContext.ProductImages.Add(productImage);

    public Task<List<ProductImage>> GetAllByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    ) =>
        dbContext
            .ProductImages.Where(pi => pi.ProductId == productId)
            .OrderByDescending(pi => pi.IsMain)
            .ThenBy(pi => pi.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<ProductImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => dbContext.ProductImages.FirstOrDefaultAsync(pi => pi.Id == id, cancellationToken);

    public Task<ProductImage?> GetMainByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    ) =>
        dbContext.ProductImages.FirstOrDefaultAsync(
            pi => pi.ProductId == productId && pi.IsMain,
            cancellationToken
        );

    public void Update(ProductImage productImage) => dbContext.ProductImages.Update(productImage);
}
