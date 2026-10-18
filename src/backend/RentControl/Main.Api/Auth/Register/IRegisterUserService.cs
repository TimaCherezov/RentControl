namespace Main.Api.Auth.Register;

public interface IRegisterUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
}
