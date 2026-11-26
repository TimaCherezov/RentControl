using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class RefreshTokenBuilder
{
    private static readonly DateTime FarFuture = new(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime FarPast = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private Guid _id = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();
    private string _tokenHash = "token-hash";
    private DateTime _expiresAt = FarFuture;
    private DateTime? _revokedAt;

    public RefreshTokenBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public RefreshTokenBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public RefreshTokenBuilder WithTokenHash(string tokenHash)
    {
        _tokenHash = tokenHash;
        return this;
    }

    public RefreshTokenBuilder WithExpiresAt(DateTime expiresAt)
    {
        _expiresAt = expiresAt;
        return this;
    }

    public RefreshTokenBuilder WithRevokedAt(DateTime revokedAt)
    {
        _revokedAt = revokedAt;
        return this;
    }

    public RefreshTokenBuilder Expired()
    {
        _expiresAt = FarPast;
        return this;
    }

    public RefreshTokenBuilder Revoked()
    {
        _revokedAt = FarPast;
        return this;
    }

    public RefreshToken Build() => new()
    {
        Id = _id,
        UserId = _userId,
        TokenHash = _tokenHash,
        ExpiresAt = _expiresAt,
        RevokedAt = _revokedAt,
    };
}
