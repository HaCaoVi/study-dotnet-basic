using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using project_basic.Common;
using project_basic.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace project_basic.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleException(context, e);
        }
    }
    private static async Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        int statusCode = (int)HttpStatusCode.InternalServerError;
        string message = "Internal Server Error";
        object? errors = null;
        
        switch (ex)
        {
            case ValidationException validationEx:
                statusCode = 400;
                message = "Validation Error";
                errors = validationEx.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                });
                break;
            case NotFoundException:
                statusCode = 404;
                message = ex.Message;
                break;

            case BadRequestException:
                statusCode = 400;
                message = ex.Message;
                break;

            case UnauthorizedException:
                statusCode = 401;
                message = ex.Message;
                break;
            default:
                errors = ex.Message;
                break;
        }

        var response = new ApiResponse<object>(
            data: null,
            message: message,
            statusCode: statusCode,
            errors: errors
        );

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(response);
    }
}