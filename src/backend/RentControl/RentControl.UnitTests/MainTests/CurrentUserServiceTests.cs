using System.Security.Claims;
using Main.Api.Exceptions;
using Main.Api.Features.Auth.CurrentUser;
using Microsoft.AspNetCore.Http;
using Moq;

namespace RentControl.UnitTests.MainTests;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _accessor = new();

    [Fact]
    public void GetUserId_WhenNameIdentifierClaimIsValid_ReturnsUserId()
    {
        var userId = Guid.NewGuid();
        _accessor.Setup(a => a.HttpContext).Returns(CreateHttpContext(new Claim(ClaimTypes.NameIdentifier, userId.ToString())));
        var service = new CurrentUserService(_accessor.Object);

        var result = service.GetUserId();

        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_WhenClaimIsMissing_ThrowsUnauthorizedRequestException()
    {
        _accessor.Setup(a => a.HttpContext).Returns(CreateHttpContext());
        var service = new CurrentUserService(_accessor.Object);

        Assert.Throws<UnauthorizedRequestException>(() => service.GetUserId());
    }

    [Fact]
    public void GetUserId_WhenClaimIsNotGuid_ThrowsUnauthorizedRequestException()
    {
        _accessor.Setup(a => a.HttpContext).Returns(CreateHttpContext(new Claim(ClaimTypes.NameIdentifier, "not-a-guid")));
        var service = new CurrentUserService(_accessor.Object);

        Assert.Throws<UnauthorizedRequestException>(() => service.GetUserId());
    }

    private static DefaultHttpContext CreateHttpContext(params Claim[] claims)
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        return context;
    }
}
