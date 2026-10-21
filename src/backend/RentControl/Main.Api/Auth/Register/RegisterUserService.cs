using Main.Model;
using Main.Model.Models;
using Microsoft.AspNetCore.Identity;

namespace Main.Api.Auth.Register;

public class RegisterUserService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher) : IRegisterUserService
{
    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email;
        var isExisted = await unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
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

        unitOfWork.Users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.Id, user.Name, user.Email);
    }
}
