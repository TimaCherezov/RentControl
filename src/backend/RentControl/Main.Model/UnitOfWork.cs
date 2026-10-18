using Main.Model.Repositories;

namespace Main.Model;

public class UnitOfWork(
    AppDbContext dbContext,
    IUserRepository users,
    IRefreshTokenRepository refreshTokens) : IUnitOfWork
{
    public IUserRepository Users => users;
    public IRefreshTokenRepository RefreshTokens => refreshTokens;

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
