namespace RentControl.Auth;

public class JwtSettings
{
    public const string Section = "Jwt";

    public string SecretKey { get; set; } = null!;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
