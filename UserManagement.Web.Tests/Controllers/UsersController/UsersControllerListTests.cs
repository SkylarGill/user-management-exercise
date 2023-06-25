using FluentValidation;
using UserManagement.Models;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.UsersController;

public class UsersControllerListTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();
    private readonly Mock<IAuditLogService> _auditLogService = new();

    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        var users = UsersControllerTestHelpers.SetupUsers(_userService);

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_WhenSpecifyingActiveFilterType_ModelMustOnlyContainActiveUsers()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupUsers(_userService);

        // Act
        var result = controller.List(FilterType.Active);

        // Assert
        _userService.Verify(service => service.FilterByActive(true), Times.Once);
        result.Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.IsActive.Should().BeTrue());
    }

    [Fact]
    public void List_WhenSpecifyingNonActiveFilterType_ModelMustOnlyContainNonActiveUsers()
    {
        // Arrange
        var controller = UsersControllerTestHelpers.CreateController(
            _userService,
            _auditLogService,
            _createUserViewModelValidator,
            _editUserViewModelValidator);
        UsersControllerTestHelpers.SetupUsers(_userService);

        // Act
        var result = controller.List(FilterType.NonActive);

        // Assert
        _userService.Verify(service => service.FilterByActive(false), Times.Once);
        result.Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.IsActive.Should().BeFalse());
    }
}