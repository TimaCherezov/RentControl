using Microsoft.AspNetCore.Http;

namespace RentControl.ServiceDefaults.Exceptions;

public abstract class ApiException(
    string message,
    int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
