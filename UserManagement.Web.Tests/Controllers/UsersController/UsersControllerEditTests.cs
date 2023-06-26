using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.UsersController;

public class UsersControllerEditTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();
    private readonly Mock<IAuditLogService> _auditLogService = new();

    [Fact]
    public async Task Edit_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupUsers(_userService);
        const long userId = 999;
        var userViewModel = new EditUserViewModel { Id = userId };

        // Act
        var result = await controller.Edit(userId, userViewModel).ConfigureAwait(false);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("UserNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(userId);
    }

    [Fact]
    public async Task Edit_WhenRequestingWithHasValidationErrorsValueOfFalse_ReturnsViewResultWithEditUserModel()
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
        var user = UsersControllerTestHelpers.SetupUsers(_userService).First();

        var viewModel = new EditUserViewModel();
        var expectedViewModel = new EditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
        };

        // Act
        var result = await controller.Edit(user.Id, viewModel).ConfigureAwait(false);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<EditUserViewModel>()
            .And.BeEquivalentTo(expectedViewModel);
    }

    [Fact]
    public async Task Edit_WhenRequestingWithHasValidationErrorsValueOfTrue_ReturnsViewResultWithProvidedEditUserViewModel()
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

        var viewModel = new EditUserViewModel()
        {
            Id = 1,
            Forename = "Test",
            Surname = "User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = true,
            HasValidationErrors = true
        };

        // Act
        var result = await controller.Edit(viewModel.Id, viewModel).ConfigureAwait(false);

        // Assert
        _userService.Verify(service => service.GetUserById(viewModel.Id), Times.Never);
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<EditUserViewModel>()
            .And.BeSameAs(viewModel);
    }
}