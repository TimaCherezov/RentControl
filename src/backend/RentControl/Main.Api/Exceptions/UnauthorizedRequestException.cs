namespace Main.Api.Exceptions;

public class UnauthorizedRequestException(string message)
    : ApiException(message, StatusCodes.Status401Unauthorized);
