using Main.Model.Repositories;

namespace Main.Model;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
