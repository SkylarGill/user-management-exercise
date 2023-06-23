using System.Linq;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Controllers;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

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
        var controller = CreateController();
        SetupUsers();

        var result = controller.List(FilterType.Active);
        
        _userService.Verify(service => service.FilterByActive(true), Times.Once);
        result.Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.IsActive.Should().BeTrue());
    }
    
    [Fact]
    public void List_WhenSpecifyingNonActiveFilterType_ModelMustOnlyContainNonActiveUsers()
    {
        var controller = CreateController();
        SetupUsers();

        var result = controller.List(FilterType.NonActive);
        
        _userService.Verify(service => service.FilterByActive(false), Times.Once);
        result.Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.IsActive.Should().BeFalse());
    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            },
            new User
            {
                Forename = "David",
                Surname = "NonActive",
                Email = "inactive@example.com",
                IsActive = false
            },
            new User
            {
                Forename = "Sarah",
                Surname = "Active",
                Email = "active@example.com",
                IsActive = true
            },
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        _userService
            .Setup(s => s.FilterByActive(true))
            .Returns(users.Where(user => user.IsActive));
        
        _userService
            .Setup(s => s.FilterByActive(false))
            .Returns(users.Where(user => !user.IsActive));

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
