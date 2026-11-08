using Main.Model.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

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

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => dbContext.Database.BeginTransactionAsync(cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
