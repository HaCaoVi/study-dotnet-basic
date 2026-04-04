using FluentValidation;
using project_basic.Dtos.UserDtos;

namespace project_basic.Validators;

public class QueryUserValidator: AbstractValidator<QueryUserDto>
{
    public QueryUserValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be >= 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}