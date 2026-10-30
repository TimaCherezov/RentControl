using Main.Model.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Main.Model.Repositories.Interfaces;

namespace Main.Model.UnitOfWork;

public interface IMainUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IProductRepository Products { get; }
    ILocationRepository Locations { get; }
    IBookingRepository Bookings { get; }
}
