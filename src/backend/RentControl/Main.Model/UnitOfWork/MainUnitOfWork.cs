using Main.Model.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Main.Model.UnitOfWork;

internal sealed class MainUnitOfWork(MainDbContext dbContext) : IMainUnitOfWork
{
    public IUserRepository Users { get; } = new UserRepository(dbContext);
    public IRefreshTokenRepository RefreshTokens { get; } = new RefreshTokenRepository(dbContext);
    public IProductRepository Products { get; } = new ProductRepository(dbContext);
    public ILocationRepository Locations { get; } = new LocationRepository(dbContext);
    public IBookingRepository Bookings { get; } = new BookingRepository(dbContext);
}
