namespace project_basic.Common;

public static class ResponseFactory
{
    public static ApiResponse<T> Response<T>(T data, string message = "Success", int statusCode = 200, object errors = null)
    {
        return new ApiResponse<T>(data, message, statusCode, errors);
    }
}