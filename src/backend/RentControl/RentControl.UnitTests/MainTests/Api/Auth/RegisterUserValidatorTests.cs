using Main.Api.Features.Auth.Register;

namespace RentControl.UnitTests.MainTests.Api.Auth;

public class RegisterUserValidatorTests
{
    private readonly RegisterUserValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsAreValid_Succeeds()
    {
        var request = new RegisterUserRequest("Ivan", "ivan@example.com", "password");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_IsInvalid()
    {
        var request = new RegisterUserRequest("", "ivan@example.com", "password");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenNameIsLongerThan200_IsInvalid()
    {
        var request = new RegisterUserRequest(new string('a', 201), "ivan@example.com", "password");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_IsInvalid()
    {
        var request = new RegisterUserRequest("Ivan", "not-an-email", "password");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_IsInvalid()
    {
        var request = new RegisterUserRequest("Ivan", "ivan@example.com", "");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
