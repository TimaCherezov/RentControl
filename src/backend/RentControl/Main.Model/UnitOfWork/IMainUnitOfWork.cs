using Main.Model.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
