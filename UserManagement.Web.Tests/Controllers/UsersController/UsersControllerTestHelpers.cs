using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentValidation;
using FluentValidation.Results;
using UserManagement.Data.Entities;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.UsersController;

public static class UsersControllerTestHelpers
{
    public static void SetupValidation(
        Mock<IValidator<CreateUserViewModel>> createUserViewModelValidator,
        Mock<IValidator<EditUserViewModel>> editUserViewModelValidator,
        bool shouldPassValidation = true,
        string propertyName = "",
        string createValidationMessage = "",
        string editValidationMessage = "")
    {
        createUserViewModelValidator
            .Setup(
                validator => validator.Validate(It.IsAny<CreateUserViewModel>()))
            .Returns(
                new ValidationResult(
                    shouldPassValidation
                        ? new List<ValidationFailure>()
                        : new List<ValidationFailure> { new(propertyName, createValidationMessage) }));

        editUserViewModelValidator
            .Setup(
                validator => validator.ValidateAsync(It.IsAny<EditUserViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ValidationResult(
                    shouldPassValidation
                        ? new List<ValidationFailure>()
                        : new List<ValidationFailure> { new(propertyName, editValidationMessage) }));
    }

    public static User[] SetupUsers(Mock<IUserService> userService)
    {
        var users = new[]
        {
            new User
            {
                Id = 1,
                Forename = "Johnny",
                Surname = "User",
                Email = "juser@example.com",
                IsActive = true,
                DateOfBirth = new DateTime(1990, 6, 24)
            },
            new User
            {
                Id = 2,
                Forename = "David",
                Surname = "NonActive",
                Email = "inactive@example.com",
                IsActive = false,
                DateOfBirth = new DateTime(1984, 12, 1)
            },
            new User
            {
                Id = 3,
                Forename = "Sarah",
                Surname = "Active",
                Email = "active@example.com",
                IsActive = true,
                DateOfBirth = new DateTime(1963, 7, 14)
            },
        };

        userService
            .Setup(s => s.GetAll())
            .ReturnsAsync(users);

        userService
            .Setup(s => s.FilterByActive(true))
            .ReturnsAsync(users.Where(user => user.IsActive));

        userService
            .Setup(s => s.FilterByActive(false))
            .ReturnsAsync(users.Where(user => !user.IsActive));

        var firstUser = users.First();

        userService
            .Setup(s => s.GetUserById(firstUser.Id)).ReturnsAsync(firstUser);
        userService
            .Setup(s => s.GetUserById(999)).ReturnsAsync(null as User);

        return users;
    }

    public static Web.Controllers.UsersController CreateController(
        Mock<IUserService> userService,
        Mock<IAuditLogService> auditLogService,
        Mock<IValidator<CreateUserViewModel>> createUserViewModelValidator,
        Mock<IValidator<EditUserViewModel>> editUserViewModelValidator) =>
        new(
            userService.Object,
            createUserViewModelValidator.Object,
            editUserViewModelValidator.Object,
            auditLogService.Object);
}