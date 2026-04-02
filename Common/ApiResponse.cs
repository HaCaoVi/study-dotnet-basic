namespace project_basic.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public object? Errors { get; set; }

    public ApiResponse(T? data = default, string message = "Success", int statusCode = 200, object? errors = null)
    {
        Data = data;
        Message = message;
        StatusCode = statusCode;
        Errors = errors;
    }

    public static ApiResponse<T> Success(T data, string message = "Success") 
        => new(data, message, 200);

    public static ApiResponse<T> Failure(int statusCode, string message, object? errors = null) 
        => new(default, message, statusCode, errors);
}