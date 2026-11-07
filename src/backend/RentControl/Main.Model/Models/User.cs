namespace Main.Model.Models;

/// <summary>
/// Пользователь.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public RoleEnum Role { get; set; } = RoleEnum.Client;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
