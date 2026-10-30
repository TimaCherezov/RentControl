using Main.Model.Repositories;

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
}
