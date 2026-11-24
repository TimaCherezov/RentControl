namespace Main.Api.Exceptions;

public sealed class ConflictException(string message)
    : ApiException(message, StatusCodes.Status409Conflict);
