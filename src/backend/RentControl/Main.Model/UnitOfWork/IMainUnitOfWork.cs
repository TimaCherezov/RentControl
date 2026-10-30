using Main.Model.Repositories;

namespace Main.Model.UnitOfWork;

public interface IMainUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IProductRepository Products { get; }
    ILocationRepository Locations { get; }
    IBookingRepository Bookings { get; }
    IProductImageRepository ProductImages { get; }
    IProductUnitRepository ProductUnits { get; }
}
