using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public class UnprocessableEntityException(string message)
    : ApiException(message, StatusCodes.Status422UnprocessableEntity);
