using EFCore.PostgresExtensions.Extensions;
using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductUnitRepository(MainDbContext dbContext) : IProductUnitRepository
{
    public void Add(ProductUnit productUnit) => dbContext.ProductUnits.Add(productUnit);

    public void Remove(ProductUnit productUnit) => dbContext.ProductUnits.Remove(productUnit);

    /// <summary>
    /// Нет проверки, что метод выполняется внутри транзакции.
    /// Использовать только внутри UOW.
    /// </summary>
    public Task<ProductUnit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        dbContext
            .ProductUnits.Where(pu => pu.Id == id)
            .ForUpdate()
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<ProductUnit>> GetAvailableAsync(
        Guid productId,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext
            .ProductUnits.AsNoTracking()
            .Where(pu => pu.ProductId == productId)
            .Where(pu => StatesExtensions.IsRentable(pu.State))
            .Where(pu =>
                !dbContext.Bookings.Any(b =>
                    b.ProductUnitId == pu.Id
                    && BookingStatusExtensions.IsBlocking(b.BookingStatus)
                    && b.StartAt < end
                    && b.EndAt > start
                )
            )
            .ToListAsync(cancellationToken);
    }
}
