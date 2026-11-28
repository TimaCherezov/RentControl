using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.Auth;
using Main.Api.Features.Auth.Refresh;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;
using RentControl.ServiceDefaults;

namespace RentControl.UnitTests.MainTests.Api.Auth;

public class RefreshUserConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<IJwtTokenService> _jwt = new();

    private RefreshToken? _addedToken;

    public RefreshUserConsumerTests()
    {
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _uow.SetupGet(u => u.RefreshTokens).Returns(_refreshTokens.Object);
        _factory.RunsWith<TokenPairResponse>(_uow);
        _jwt.Setup(j => j.GenerateToken(It.IsAny<Guid>(), It.IsAny<RoleEnum>())).Returns("new-access-token");
        _jwt.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh-token");
        _jwt.Setup(j => j.HashRefreshToken("refresh-token")).Returns("token-hash");
        _jwt.Setup(j => j.HashRefreshToken("new-refresh-token")).Returns("hashed-new-token");
        _refreshTokens
            .Setup(r => r.Add(It.IsAny<RefreshToken>()))
            .Callback<RefreshToken>(token => _addedToken = token);
    }

    [Fact]
    public async Task Consume_WhenTokenNotFound_ThrowsUnauthorizedRequestException()
    {
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RefreshUserRequest("refresh-token"));

        await Assert.ThrowsAsync<UnauthorizedRequestException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenTokenIsRevoked_ThrowsUnauthorizedRequestException()
    {
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshTokenBuilder().Revoked().Build());
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RefreshUserRequest("refresh-token"));

        await Assert.ThrowsAsync<UnauthorizedRequestException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenTokenIsExpired_ThrowsUnauthorizedRequestException()
    {
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshTokenBuilder().Expired().Build());
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RefreshUserRequest("refresh-token"));

        await Assert.ThrowsAsync<UnauthorizedRequestException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenTokenIsValid_RotatesTokensAndRespondsWithNewPair()
    {
        var user = new UserBuilder().Build();
        var currentToken = new RefreshTokenBuilder().WithUserId(user.Id).Build();
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentToken);
        _users
            .Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RefreshUserRequest("refresh-token"));
        TokenPairResponse? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<TokenPairResponse>()))
            .Callback<TokenPairResponse>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(currentToken.RevokedAt);
        Assert.NotNull(_addedToken);
        Assert.Equal("hashed-new-token", _addedToken.TokenHash);
        Assert.True(_addedToken.ExpiresAt > DateTime.UtcNow);
        Assert.Equal("new-access-token", responded?.AccessToken);
        Assert.Equal("new-refresh-token", responded?.RefreshToken);
    }

    private RefreshUserConsumer CreateConsumer() => new(
        _factory.Object,
        _jwt.Object,
        Options.Create(new JwtOptions()));
}
