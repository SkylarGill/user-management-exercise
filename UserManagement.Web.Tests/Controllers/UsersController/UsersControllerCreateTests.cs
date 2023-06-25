using System;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Entities;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.UsersController;

public class UsersControllerCreateTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();
    private readonly Mock<IAuditLogService> _auditLogService = new();


    [Fact]
    public void Create_WhenGettingPage_ReturnsViewModel()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);

        // Act
        var result = controller.Create();

        // Assert
        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeNull();
    }

    [Fact]
    public void Create_WhenSubmittingUserWithNoValidationErrors_CallsCreateUserAndRedirectsToListAction()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupValidation(
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        var viewModel = new CreateUserViewModel()
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = true
        };

        // Act
        var result = controller.Create(viewModel);

        // Assert
        _userService.Verify(service => service.CreateUser(It.IsAny<User>()), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("List");
    }

    [Fact]
    public void Create_WhenSubmittingUserWithValidationErrors_ReturnsViewResult()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        const string createValidationMessage = "CreateUserViewModel validation failure";
        UsersControllerTestHelpers.SetupValidation(
            _createUserViewModelValidator,
            _editUserViewModelValidator,
            false,
            propertyName: nameof(CreateUserViewModel.Email),
            createValidationMessage: createValidationMessage,
            editValidationMessage: "EditUserViewModel validation failure");
        var viewModel = new CreateUserViewModel()
        {
            Forename = "Test",
            Surname = "User",
            Email = "invalidemail",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = true
        };

        // Act
        var result = controller.Create(viewModel);

        // Assert
        _userService.Verify(service => service.CreateUser(It.IsAny<User>()), Times.Never);
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<CreateUserViewModel>()
            .Which.Should().BeEquivalentTo(viewModel);
    }

    [Fact]
    public void Create_WhenSubmittingUserWithValidationErrors_AddsValidationErrorsToModelState()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        const string createValidationMessage = "CreateUserViewModel validation failure";
        const string propertyName = nameof(CreateUserViewModel.Email);
        UsersControllerTestHelpers.SetupValidation(
            _createUserViewModelValidator,
            _editUserViewModelValidator,
            false,
            propertyName: propertyName,
            createValidationMessage: createValidationMessage,
            editValidationMessage: "EditUserViewModel validation failure");
        var viewModel = new CreateUserViewModel()
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = true
        };

        // Act
        controller.Create(viewModel);

        // Assert
        controller.ModelState
            .Should().ContainKey(propertyName)
            .WhoseValue?.Errors.Should().HaveCount(1).And
            .Contain(error => error.ErrorMessage == createValidationMessage);
    }
}