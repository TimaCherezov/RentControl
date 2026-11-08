using Main.Model.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.UnitOfWork;

internal sealed class MainUnitOfWork(MainDbContext dbContext) : IMainUnitOfWork
{
    public IUserRepository Users { get; } = new UserRepository(dbContext);
    public IRefreshTokenRepository RefreshTokens { get; } = new RefreshTokenRepository(dbContext);
    public IProductRepository Products { get; } = new ProductRepository(dbContext);
    public ILocationRepository Locations { get; } = new LocationRepository(dbContext);
    public IBookingRepository Bookings { get; } = new BookingRepository(dbContext);
    public IProductImageRepository ProductImages { get; } = new ProductImageRepository(dbContext);
    public IProductUnitRepository ProductUnits { get; } = new ProductUnitRepository(dbContext);

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default
    )
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync<T>(
            async (cancellationToken) =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(
                    cancellationToken
                );
                try
                {
                    var result = await operation();
                    await dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            },
            cancellationToken
        );
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
