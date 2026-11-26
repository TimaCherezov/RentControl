using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Main.Api.Features.Auth;
using Main.Model.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RentControl.ServiceDefaults;

namespace RentControl.UnitTests.MainTests;

public class JwtTokenServiceTests : IDisposable
{
    private static readonly string ExpectedTokenHash =
        "TF3Jt3CJBfd/Xl0WMWtd+0JeaMsybc1VqGDpCncHAx4=";

    private readonly RSA _rsa = RSA.Create(2048);
    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        var options = new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            PrivateKeyPem = _rsa.ExportPkcs8PrivateKeyPem(),
        };
        _service = new JwtTokenService(
            new JwtSigningKeyProvider(Options.Create(options)),
            Options.Create(options));
    }

    public void Dispose() => _rsa.Dispose();

    [Fact]
    public void GenerateToken_WhenValidInput_ContainsUserIdAndRoleClaims()
    {
        var userId = Guid.NewGuid();

        var token = _service.GenerateToken(userId, RoleEnum.Admin);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(
            userId.ToString(),
            jwt.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(
            nameof(RoleEnum.Admin),
            jwt.Claims.Single(claim => claim.Type == ClaimTypes.Role).Value);
    }

    [Fact]
    public void GenerateToken_WhenIssuerAndAudienceConfigured_SetsThem()
    {
        var token = _service.GenerateToken(Guid.NewGuid(), RoleEnum.Client);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("test-issuer", jwt.Issuer);
        Assert.Equal("test-audience", jwt.Audiences.Single());
    }

    [Fact]
    public void GenerateToken_WhenIssuerAndAudienceAreEmpty_OmitsThem()
    {
        var options = new JwtOptions
        {
            Issuer = string.Empty,
            Audience = string.Empty,
            PrivateKeyPem = _rsa.ExportPkcs8PrivateKeyPem(),
        };
        var service = new JwtTokenService(
            new JwtSigningKeyProvider(Options.Create(options)),
            Options.Create(options));

        var token = service.GenerateToken(Guid.NewGuid(), RoleEnum.Client);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Null(jwt.Issuer);
        Assert.Empty(jwt.Audiences);
    }

    [Fact]
    public void GenerateRefreshToken_WhenCalled_Returns32BytesEncodedAsBase64Url()
    {
        var refreshToken = _service.GenerateRefreshToken();

        var bytes = WebEncoders.Base64UrlDecode(refreshToken);
        Assert.Equal(32, bytes.Length);
    }

    [Fact]
    public void GenerateRefreshToken_WhenCalledTwice_ReturnsDifferentTokens()
    {
        var first = _service.GenerateRefreshToken();
        var second = _service.GenerateRefreshToken();

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void HashRefreshToken_WhenToken_ReturnsSha256AsBase64()
    {
        var hash = _service.HashRefreshToken("test-token");

        Assert.Equal(ExpectedTokenHash, hash);
    }
}
