namespace RentControl.Auth;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string SecretKey { get; set; } = null!;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshTokenExpiry { get; set; } = TimeSpan.FromDays(7);
}
