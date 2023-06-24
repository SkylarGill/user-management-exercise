using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;

namespace UserManagement.Web.Tests.Controllers.UserController;

public class UsersControllerUserNotFoundTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();

    [Fact]
    public void UserNotFound_WhenRequestingWithUserId_ReturnsViewResultWithIdInModel()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        var viewModel = new UserNotFoundViewModel(99);

        // Act
        var result = controller.UserNotFound(viewModel.Id);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserNotFoundViewModel>()
            .And.BeEquivalentTo(viewModel);
    }
}