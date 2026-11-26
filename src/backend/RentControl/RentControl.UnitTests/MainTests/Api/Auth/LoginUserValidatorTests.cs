using Main.Api.Features.Auth.Login;

namespace RentControl.UnitTests.MainTests.Api.Auth;

public class LoginUserValidatorTests
{
    private readonly LoginUserValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsAreValid_Succeeds()
    {
        var request = new LoginUserRequest("ivan@example.com", "password");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEmailIsEmpty_IsInvalid()
    {
        var request = new LoginUserRequest("", "password");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_IsInvalid()
    {
        var request = new LoginUserRequest("not-an-email", "password");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_IsInvalid()
    {
        var request = new LoginUserRequest("ivan@example.com", "");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
