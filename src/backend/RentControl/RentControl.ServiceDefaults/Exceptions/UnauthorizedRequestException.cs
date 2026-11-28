using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public class UnauthorizedRequestException(string message)
    : ApiException(message, StatusCodes.Status401Unauthorized);
