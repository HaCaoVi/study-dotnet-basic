using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using project_basic.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace project_basic.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.Any())
        {
            throw new BadRequestException("Request body is required");
        }
        foreach (var arg in context.ActionArguments)
        {
            if (arg.Value == null)
            {
                throw new BadRequestException("Request body is required");
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.Value.GetType());
            var validator = _serviceProvider.GetService(validatorType);

            if (validator == null) continue;

            var method = validatorType.GetMethod("ValidateAsync", new[] { arg.Value.GetType(), typeof(CancellationToken) });

            var task = (Task<ValidationResult>)method.Invoke(validator, new object[] { arg.Value, CancellationToken.None });

            var result = await task;

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        await next();
    }
}