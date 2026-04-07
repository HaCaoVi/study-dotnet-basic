using FluentValidation;
using project_basic.Dtos.UserDtos;

namespace project_basic.Validators;

public class CreateUserValidator: AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(u=> u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email not valid")
            .MaximumLength(255).WithMessage("Email max length is 255");
        
        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(50).WithMessage("Password max length is 50");

        RuleFor(u => u.Age)
            .GreaterThan(0)
            .When(u => u.Age.HasValue)
            .WithMessage("Age must be greater than 0");
        
        RuleFor(u => u.Address)
            .MaximumLength(255)
            .When(u => !string.IsNullOrEmpty(u.Address))
            .WithMessage("Address max length is 255");

        RuleFor(u => u.RoleId)
            .NotEmpty().WithMessage("RoleId is required");
    }
}