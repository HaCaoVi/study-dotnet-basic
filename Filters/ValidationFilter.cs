using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using project_basic.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;

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
        // Check for null or empty body if the action expects arguments
        var bodyParam = context.ActionDescriptor.Parameters
            .FirstOrDefault(p => p.BindingInfo?.BindingSource?.Id == "Body");

        if (bodyParam != null)
        {
            if (!context.ActionArguments.TryGetValue(bodyParam.Name, out var bodyValue) || bodyValue == null)
            {
                throw new BadRequestException("Request body is required and cannot be empty.");
            }

            // Get validator for the body type
            var validatorType = typeof(IValidator<>).MakeGenericType(bodyParam.ParameterType);
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(bodyValue);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }
            }
        }

        await next();
    }
}