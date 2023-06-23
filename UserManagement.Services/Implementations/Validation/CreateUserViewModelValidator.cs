using System;
using FluentValidation;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations.Validation;

public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
{
    public CreateUserViewModelValidator(ICurrentDateProvider dateProvider)
    {
        RuleFor(model => model.Forename)
            .NotEmpty().WithMessage("Forename must not be empty or whitespace");
        RuleFor(model => model.Surname)
            .NotEmpty().WithMessage("Surname must not be empty or whitespace");
        RuleFor(model => model.Email)
            .NotNull().WithMessage("Email must be a valid email address")
            .EmailAddress().WithMessage("Email must be a valid email address");
        RuleFor(model => model.DateOfBirth)
            .GreaterThan(default(DateTime)).WithMessage("Date of Birth must be specified")
            .LessThanOrEqualTo(dateProvider.GetCurrentDate()).WithMessage("Date of Birth cannot be in the future");
    }
}