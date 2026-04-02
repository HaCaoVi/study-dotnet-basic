namespace project_basic.Common.Exceptions;

public abstract class BaseException : Exception
{
    public int StatusCode { get; }
    public object? Errors { get; }

    protected BaseException(string message, int statusCode, object? errors = null) : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}
