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
        // 🔥 1. Check model binding error trước
        if (!context.ModelState.IsValid)
        {
            var failures = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e =>
                {
                    var field = x.Key;

                    // remove $. prefix
                    if (field.StartsWith("$."))
                        field = field.Substring(2);

                    if (field == "createUserDto")
                        return null; // bỏ root object

                    var message = e.ErrorMessage;

                    // map lỗi GUID
                    if (message.Contains("System.Guid") || message.Contains("could not be converted"))
                    {
                        message = $"{field} must be a valid GUID";
                    }

                    return new FluentValidation.Results.ValidationFailure(field, message);
                }))
                .Where(x => x != null)
                .ToList();

            throw new ValidationException(failures);
        }

        // 🔥 2. Sau đó mới check body null
        var bodyParam = context.ActionDescriptor.Parameters
            .FirstOrDefault(p => p.BindingInfo?.BindingSource?.Id == "Body");

        if (bodyParam != null)
        {
            if (!context.ActionArguments.TryGetValue(bodyParam.Name, out var bodyValue) || bodyValue == null)
            {
                throw new BadRequestException("Request body is required and cannot be empty.");
            }

            // 🔥 3. FluentValidation
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