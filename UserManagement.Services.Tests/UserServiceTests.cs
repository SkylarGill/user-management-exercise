using System;
using System.Linq;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Implementations;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    } 
    
    [Fact]
    public void FilterByActive_WhenIsActiveParamIsTrue_MustOnlyReturnEntitiesTrueIsActiveValue()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();

        // Act
        var result = service.FilterByActive(true);

        //Assert
        result.Should().AllSatisfy(model => model.IsActive.Should().BeTrue());;
    }
    
    [Fact]
    public void FilterByActive_WhenIsActiveParamIsFalse_MustOnlyReturnEntitiesWithFalseIsActiveValue()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();

        // Act
        var result = service.FilterByActive(false);

        //Assert
        result.Should().AllSatisfy(model => model.IsActive.Should().BeFalse());;
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive, 
                DateOfBirth = new DateTime(1990, 6, 24)
            },
            new User
            {
                Forename = "David",
                Surname = "NonActive",
                Email = "inactive@example.com",
                IsActive = false, 
                DateOfBirth = new DateTime(1984, 12, 1)
            },
            new User
            {
                Forename = "Sarah",
                Surname = "Active",
                Email = "active@example.com",
                IsActive = true, 
                DateOfBirth = new DateTime(1963, 7, 14)
            },
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}
