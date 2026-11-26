using Main.Api.Features.Auth;
using Main.Api.Features.Auth.Logout;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Auth;

public class LogoutUserConsumerTests
{
    private static readonly DateTime OriginalRevokedAt = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<IJwtTokenService> _jwt = new();

    public LogoutUserConsumerTests()
    {
        _uow.SetupGet(u => u.RefreshTokens).Returns(_refreshTokens.Object);
        _factory.RunsWith(_uow);
        _jwt.Setup(j => j.HashRefreshToken("refresh-token")).Returns("token-hash");
    }

    [Fact]
    public async Task Consume_WhenTokenIsActive_RevokesToken()
    {
        var token = new RefreshTokenBuilder().Build();
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LogoutUserRequest("refresh-token"));

        await consumer.Consume(context.Object);

        Assert.NotNull(token.RevokedAt);
    }

    [Fact]
    public async Task Consume_WhenTokenAlreadyRevoked_KeepsOriginalRevocation()
    {
        var token = new RefreshTokenBuilder().WithRevokedAt(OriginalRevokedAt).Build();
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LogoutUserRequest("refresh-token"));

        await consumer.Consume(context.Object);

        Assert.Equal(OriginalRevokedAt, token.RevokedAt);
    }

    [Fact]
    public async Task Consume_WhenTokenNotFound_CompletesWithoutException()
    {
        _refreshTokens
            .Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LogoutUserRequest("refresh-token"));

        await consumer.Consume(context.Object);
    }

    private LogoutUserConsumer CreateConsumer() => new(_factory.Object, _jwt.Object);
}
