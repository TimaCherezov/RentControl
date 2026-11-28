using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public abstract class ApiException(
    string message,
    int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public sealed class ConflictException(string message)
    : ApiException(message, StatusCodes.Status409Conflict);

public sealed class ForbiddenException(string message)
    : ApiException(message, StatusCodes.Status403Forbidden);

public class ObjectNotFoundException(string message)
    : ApiException(message, StatusCodes.Status404NotFound);

public class UnauthorizedRequestException(string message)
    : ApiException(message, StatusCodes.Status401Unauthorized);

public class UnprocessableEntityException(string message)
    : ApiException(message, StatusCodes.Status422UnprocessableEntity);
