namespace project_basic.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public object? Errors { get; set; }

    public ApiResponse(T? data , string message, int statusCode, object? errors = null)
    {
        Data = data;
        Message = message;
        StatusCode = statusCode;
        Errors = errors;
    }
}