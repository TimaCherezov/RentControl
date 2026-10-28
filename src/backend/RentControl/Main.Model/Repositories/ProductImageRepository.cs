using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductImageRepository(MainDbContext dbcontext) : IProductImageRepository
{
    public void Add(ProductImage productImage) => dbcontext.ProductImages.Add(productImage);

    public Task<List<ProductImage>> GetAllForProductByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    ) =>
        dbcontext
            .ProductImages.Where(pi => pi.ProductId == productId)
            .ToListAsync(cancellationToken);

    public Task<ProductImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => dbcontext.ProductImages.FirstOrDefaultAsync(pi => pi.Id == id, cancellationToken);

    public Task<ProductImage?> GetMainForProductByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    ) =>
        dbcontext.ProductImages.FirstOrDefaultAsync(
            pi => pi.ProductId == productId && pi.IsMain,
            cancellationToken
        );

    public void Update(ProductImage productImage) => dbcontext.ProductImages.Update(productImage);
}
