namespace Main.Api.Exceptions;

public class UnprocessableEntityException(string message)
    : ApiException(message, StatusCodes.Status422UnprocessableEntity);
