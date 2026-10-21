namespace Main.Api.Auth.Login;

public interface ILoginUserService
{
    Task<TokenPairResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
}
