using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Auth.Register;

public class RegisterUserConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IPasswordHasher<User> passwordHasher) : IConsumer<RegisterUserRequest>
{
    public async Task Consume(ConsumeContext<RegisterUserRequest> context)
    {
        var response = await uowFactory.ExecuteAsync(async uow =>
        {
            var request = context.Message;
            var existingUser = await uow.Users.GetByEmailAsync(
                request.Email,
                context.CancellationToken);
            if (existingUser is not null)
            {
                throw new ConflictException($"User {request.Email} already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                PasswordHash = null!,
                Name = request.Name.Trim(),
                Email = request.Email,
                Role = RoleEnum.Client
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            uow.Users.Add(user);

            return new UserResponse(user.Id, user.Name, user.Email);
        }, context.CancellationToken);

        await context.RespondAsync(response);
    }
}
