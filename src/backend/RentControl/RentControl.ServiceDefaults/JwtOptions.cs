namespace RentControl.ServiceDefaults;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string? PrivateKeyPem { get; set; }
    public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshTokenExpiry { get; set; } = TimeSpan.FromDays(7);
}
