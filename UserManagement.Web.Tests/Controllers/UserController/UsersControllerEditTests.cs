using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;

namespace UserManagement.Web.Tests.Controllers.UserController;

public class UsersControllerEditTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();

    [Fact]
    public void Edit_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupUsers(_userService);
        const long userId = 999;
        var userViewModel = new EditUserViewModel { Id = userId };

        // Act
        var result = controller.Edit(userId, userViewModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("UserNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(userId);
    }

    [Fact]
    public void Edit_WhenRequestingWithHasValidationErrorsValueOfFalse_ReturnsViewResultWithEditUserModel()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
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
        var result = controller.Edit(user.Id, viewModel);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<EditUserViewModel>()
            .And.BeEquivalentTo(expectedViewModel);
    }

    [Fact]
    public void Edit_WhenRequestingWithHasValidationErrorsValueOfTrue_ReturnsViewResultWithProvidedEditUserViewModel()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
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
        var result = controller.Edit(viewModel.Id, viewModel);

        // Assert
        _userService.Verify(service => service.GetUserById(viewModel.Id), Times.Never);
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<EditUserViewModel>()
            .And.BeSameAs(viewModel);
    }
}