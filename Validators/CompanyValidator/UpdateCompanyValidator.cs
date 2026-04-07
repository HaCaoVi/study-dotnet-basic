using FluentValidation;
using project_basic.Dtos.CompanyDto;

namespace project_basic.Validators.CompanyValidator;

public class UpdateCompanyValidator: AbstractValidator<UpdateCompanyDto>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(255).WithMessage("Company name must not exceed 255 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}