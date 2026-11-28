using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public sealed class ConflictException(string message)
    : ApiException(message, StatusCodes.Status409Conflict);
