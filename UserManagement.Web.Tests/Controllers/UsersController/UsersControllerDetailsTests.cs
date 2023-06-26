using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.UsersController;

public class UsersControllerDetailsTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();
    private readonly Mock<IAuditLogService> _auditLogService = new();

    [Fact]
    public async Task Details_WhenRequestingExistingUserId_ReturnsViewModelWithCorrectParameters()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        var user = UsersControllerTestHelpers.SetupUsers(_userService).First();

        // Act
        var result = await controller.Details(user.Id);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserDetailsViewModel>()
            .And.BeEquivalentTo(user);
    }

    [Fact]
    public async Task Details_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupUsers(_userService);
        const long userId = 999;

        // Act
        var result = await controller.Details(userId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("UserNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(userId);
    }
}