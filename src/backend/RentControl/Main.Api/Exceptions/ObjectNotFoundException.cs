namespace Main.Api.Exceptions;

public class ObjectNotFoundException(string message)
    : ApiException(message, StatusCodes.Status404NotFound);
