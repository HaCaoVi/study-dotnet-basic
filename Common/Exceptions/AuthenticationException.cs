namespace project_basic.Common.Exceptions;

public class AuthenticationException: BaseException
{
    public AuthenticationException(string message, object? errors = null) 
        : base(message, 401, errors)
    {
    }
}
