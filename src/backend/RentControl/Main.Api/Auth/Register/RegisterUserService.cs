using Main.Model.Models;
using Main.Model.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Auth.Register;

public class RegisterUserService(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IPasswordHasher<User> passwordHasher) : IRegisterUserService
{
    public Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email;

        return uowFactory.ExecuteAsync(async uow =>
        {
            var isExisted = await uow.Users.GetByEmailAsync(email, cancellationToken);
            if (isExisted != null)
            {
                throw new InvalidOperationException($"User {email} already exists.");
            }

            var user = new User
            {
                Name = request.Name.Trim(),
                Email = email
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            uow.Users.Add(user);

            return new UserResponse(user.Id, user.Name, user.Email);
        }, cancellationToken);
    }
}
