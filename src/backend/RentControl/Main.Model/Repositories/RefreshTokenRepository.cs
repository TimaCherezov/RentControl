using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class RefreshTokenRepository(MainDbContext dbContext) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public void Add(RefreshToken refreshToken)
        => dbContext.RefreshTokens.Add(refreshToken);

    public void Update(RefreshToken refreshToken)
        => dbContext.RefreshTokens.Update(refreshToken);
}
