using Main.Api.Exceptions;
using Main.Api.Features.Auth;
using Main.Api.Features.Auth.Login;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;
using RentControl.ServiceDefaults;

namespace RentControl.UnitTests.MainTests;

public class LoginUserConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<IPasswordHasher<User>> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwt = new();

    private RefreshToken? _addedToken;

    public LoginUserConsumerTests()
    {
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _uow.SetupGet(u => u.RefreshTokens).Returns(_refreshTokens.Object);
        _factory.RunsWith<TokenPairResponse>(_uow);
        _jwt.Setup(j => j.GenerateToken(It.IsAny<Guid>(), It.IsAny<RoleEnum>())).Returns("access-token");
        _jwt.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");
        _jwt.Setup(j => j.HashRefreshToken("refresh-token")).Returns("hashed-refresh");
        _passwordHasher
            .Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        _refreshTokens
            .Setup(r => r.Add(It.IsAny<RefreshToken>()))
            .Callback<RefreshToken>(token => _addedToken = token);
    }

    [Fact]
    public async Task Consume_WhenUserNotFound_ThrowsUnauthorizedRequestException()
    {
        _users
            .Setup(u => u.GetByEmailAsync("ivan@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LoginUserRequest("ivan@example.com", "password"));

        await Assert.ThrowsAsync<UnauthorizedRequestException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenPasswordIsInvalid_ThrowsUnauthorizedRequestException()
    {
        var user = new UserBuilder().WithEmail("ivan@example.com").Build();
        _users
            .Setup(u => u.GetByEmailAsync("ivan@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LoginUserRequest("ivan@example.com", "wrong-password"));

        await Assert.ThrowsAsync<UnauthorizedRequestException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenValid_RespondsWithTokenPair()
    {
        ArrangeValidUser();
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LoginUserRequest("ivan@example.com", "password"));
        TokenPairResponse? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<TokenPairResponse>()))
            .Callback<TokenPairResponse>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.Equal("access-token", responded?.AccessToken);
        Assert.Equal("refresh-token", responded?.RefreshToken);
    }

    [Fact]
    public async Task Consume_WhenValid_StoresHashedRefreshToken()
    {
        var user = ArrangeValidUser();
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new LoginUserRequest("ivan@example.com", "password"));

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedToken);
        Assert.Equal(user.Id, _addedToken.UserId);
        Assert.Equal("hashed-refresh", _addedToken.TokenHash);
        Assert.True(_addedToken.ExpiresAt > DateTime.UtcNow);
    }

    private User ArrangeValidUser()
    {
        var user = new UserBuilder().WithEmail("ivan@example.com").Build();
        _users
            .Setup(u => u.GetByEmailAsync("ivan@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        return user;
    }

    private LoginUserConsumer CreateConsumer() => new(
        _factory.Object,
        _passwordHasher.Object,
        _jwt.Object,
        Options.Create(new JwtOptions()));
}
