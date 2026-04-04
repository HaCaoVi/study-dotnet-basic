using FluentValidation;
using project_basic.Dtos.AuthDtos;
using project_basic.Repositories.Interfaces;

namespace project_basic.Validators.AuthValidator;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}