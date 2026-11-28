using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public sealed class ForbiddenException(string message)
    : ApiException(message, StatusCodes.Status403Forbidden);
