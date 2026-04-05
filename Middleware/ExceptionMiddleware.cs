using System.Net;
using FluentValidation;
using project_basic.Common;
using project_basic.Common.Exceptions;

namespace project_basic.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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

    private async Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";
        object? errors = null;
        switch (ex)
        {
            case BaseException baseEx:
                statusCode = baseEx.StatusCode;
                message = baseEx.Message;
                errors = baseEx.Errors;
                break;

            case ValidationException validationEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Validation failed";
                errors = validationEx.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                });
                break;
            default:
                // Log the actual internal error
                _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
                
                // In production, we don't return the full exception message
                if (_env.IsDevelopment())
                {
                    message = ex.Message;
                    errors = ex.StackTrace;
                }
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