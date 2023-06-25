using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Entities;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.UsersController;

public class UsersControllerSubmitEditTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();    
    private readonly Mock<IAuditLogService> _auditLogService = new();

    [Fact]
    public void SubmitEdit_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
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
        UsersControllerTestHelpers.SetupUsers(_userService);
        const long userId = 999;
        var userViewModel = new EditUserViewModel { Id = userId };

        // Act
        var result = controller.SubmitEdit(userId, userViewModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("UserNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(userId);
    }

    [Fact]
    public void SubmitEdit_WhenSubmittingUserWithValidationErrors_ReturnsViewResult()
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
        var viewModel = new EditUserViewModel
        {
            Id = 1,
            Forename = "Test",
            Surname = "User",
            Email = "invalidemail",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = true
        };

        // Act
        var result = controller.SubmitEdit(viewModel.Id, viewModel);

        // Assert
        _userService.Verify(service => service.UpdateUser(It.IsAny<User>()), Times.Never);
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<EditUserViewModel>()
            .Which.Should().BeEquivalentTo(viewModel);
        var viewResult = result as ViewResult;
        viewResult?.ViewName.Should().BeEquivalentTo("Edit");
    }

    [Fact]
    public void SubmitEdit_WhenSubmittingUserWithValidationErrors_AddsValidationErrorsToModelState()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        const string editValidationMessage = "EditUserViewModel validation failure";
        const string propertyName = nameof(CreateUserViewModel.Email);
        UsersControllerTestHelpers.SetupValidation(
            _createUserViewModelValidator,
            _editUserViewModelValidator,
            false,
            propertyName: propertyName,
            createValidationMessage: "CreateUserViewModel validation failure",
            editValidationMessage: editValidationMessage);

        var viewModel = new EditUserViewModel
        {
            Id = 1,
            Forename = "Test",
            Surname = "User",
            Email = "invalidemail",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = true
        };

        // Act
        controller.SubmitEdit(viewModel.Id, viewModel);

        // Assert
        controller.ModelState
            .Should().ContainKey(propertyName)
            .WhoseValue?.Errors.Should().HaveCount(1).And
            .Contain(error => error.ErrorMessage == editValidationMessage);
    }

    [Fact]
    public void SubmitEdit_WhenSubmittingUserWithNoValidationErrors_CallsUpdateUserAndRedirectsToListAction()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupValidation(_createUserViewModelValidator, _editUserViewModelValidator);
        var user = UsersControllerTestHelpers.SetupUsers(_userService).First();

        var viewModel = new EditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
        };

        // Act
        var result = controller.SubmitEdit(viewModel.Id, viewModel);

        // Assert
        _userService.Verify(service => service.UpdateUser(It.Is<User>(u => u.Id == user.Id)), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("List");
    }
}