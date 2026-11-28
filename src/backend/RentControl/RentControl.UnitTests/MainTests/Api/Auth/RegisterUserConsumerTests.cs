using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.Auth;
using Main.Api.Features.Auth.Register;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Auth;

public class RegisterUserConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<IPasswordHasher<User>> _passwordHasher = new();

    private User? _addedUser;

    public RegisterUserConsumerTests()
    {
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _factory.RunsWith<UserResponse>(_uow);
        _passwordHasher
            .Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns("hashed-password");
        _users
            .Setup(u => u.Add(It.IsAny<User>()))
            .Callback<User>(user => _addedUser = user);
    }

    [Fact]
    public async Task Consume_WhenEmailAlreadyExists_ThrowsConflictException()
    {
        var email = "ivan@example.com";
        _users
            .Setup(u => u.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserBuilder().WithEmail(email).Build());
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RegisterUserRequest("Ivan", email, "password"));

        await Assert.ThrowsAsync<ConflictException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenValid_AddsClientUserWithTrimmedNameAndHashedPassword()
    {
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RegisterUserRequest("  Ivan  ", "ivan@example.com", "password"));

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedUser);
        Assert.Equal("Ivan", _addedUser.Name);
        Assert.Equal(RoleEnum.Client, _addedUser.Role);
        Assert.Equal("hashed-password", _addedUser.PasswordHash);
    }

    [Fact]
    public async Task Consume_WhenValid_RespondsWithCreatedUser()
    {
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new RegisterUserRequest("Ivan", "ivan@example.com", "password"));
        UserResponse? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<UserResponse>()))
            .Callback<UserResponse>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(_addedUser?.Id, responded.Id);
        Assert.Equal("Ivan", responded.Name);
        Assert.Equal("ivan@example.com", responded.Email);
    }

    private RegisterUserConsumer CreateConsumer() => new(_factory.Object, _passwordHasher.Object);
}
