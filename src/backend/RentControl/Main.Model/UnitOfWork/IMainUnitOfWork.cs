using Main.Model.Repositories;

namespace Main.Model.UnitOfWork;

public interface IMainUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IProductRepository Products { get; }
    ILocationRepository Locations { get; }
    IBookingRepository Bookings { get; }
    IBookingStatusRepository BookingStatuses { get; }
    IProductImageRepository ProductImages { get; }
    IStateRepository States { get; }
    IProductUnitRepository ProductUnits { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
