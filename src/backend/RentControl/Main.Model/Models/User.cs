namespace Main.Model.Models;

public class User
{
    public Guid Id { get; set; }
    public required Role Role { get; set; }
    public Guid RoleId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
