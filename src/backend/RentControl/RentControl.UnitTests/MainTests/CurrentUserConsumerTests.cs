using Main.Api.Exceptions;
using Main.Api.Features.Auth;
using Main.Api.Features.Auth.CurrentUser;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class CurrentUserConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    public CurrentUserConsumerTests()
    {
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _factory.RunsWith<UserResponse>(_uow);
    }

    [Fact]
    public async Task Consume_WhenUserExists_RespondsWithUserResponse()
    {
        var userId = Guid.NewGuid();
        var user = new UserBuilder().WithId(userId).Build();
        _currentUser.Setup(s => s.GetUserId()).Returns(userId);
        _users
            .Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CurrentUserRequest());
        UserResponse? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<UserResponse>()))
            .Callback<UserResponse>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(user.Id, responded.Id);
        Assert.Equal(user.Name, responded.Name);
        Assert.Equal(user.Email, responded.Email);
    }

    [Fact]
    public async Task Consume_WhenUserNotFound_ThrowsUnauthorizedRequestException()
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(s => s.GetUserId()).Returns(userId);
        _users
            .Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CurrentUserRequest());

        await Assert.ThrowsAsync<UnauthorizedRequestException>(() => consumer.Consume(context.Object));
    }

    private CurrentUserConsumer CreateConsumer() => new(_currentUser.Object, _factory.Object);
}
