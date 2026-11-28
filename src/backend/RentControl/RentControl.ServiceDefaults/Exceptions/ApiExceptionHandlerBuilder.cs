namespace RentControl.ServiceDefaults.Exceptions;

public sealed record ApiError(string Field, string Error);

public sealed record ApiErrorResponse(int StatusCode, IReadOnlyList<ApiError> Errors);

public sealed class ApiExceptionHandlerBuilder
{
    private readonly List<Func<Exception, Exception>> _unwrappers = [];
    private readonly List<Func<Exception, ApiErrorResponse?>> _mappers = [];

    public ApiExceptionHandlerBuilder Unwrap(Func<Exception, Exception> unwrapper)
    {
        _unwrappers.Add(unwrapper);
        return this;
    }

    public ApiExceptionHandlerBuilder Unwrap<TException>() where TException : Exception
    {
        return Unwrap(ex => ex is TException outer && outer.InnerException is { } inner ? inner : ex);
    }

    public ApiExceptionHandlerBuilder Map(Func<Exception, ApiErrorResponse?> mapper)
    {
        _mappers.Add(mapper);
        return this;
    }

    public ApiExceptionHandlerBuilder Map<TException>(Func<TException, ApiErrorResponse?> mapper)
        where TException : Exception
    {
        return Map(ex => ex is TException typed ? mapper(typed) : null);
    }

    public ApiExceptionHandlerBuilder Map<TException>(int statusCode, Func<TException, string> messageSelector)
        where TException : Exception
    {
        return Map<TException>(ex => new ApiErrorResponse(
            statusCode,
            [new ApiError(string.Empty, messageSelector(ex))]));
    }

    internal Exception UnwrapException(Exception exception)
    {
        var current = exception;
        foreach (var unwrapper in _unwrappers)
        {
            current = unwrapper(current);
        }

        return current;
    }

    internal ApiErrorResponse? MapException(Exception exception)
    {
        foreach (var mapper in _mappers)
        {
            if (mapper(exception) is { } response)
            {
                return response;
            }
        }

        return null;
    }
}
