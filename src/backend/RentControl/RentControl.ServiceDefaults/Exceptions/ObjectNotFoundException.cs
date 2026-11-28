using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public class ObjectNotFoundException(string message)
    : ApiException(message, StatusCodes.Status404NotFound);
