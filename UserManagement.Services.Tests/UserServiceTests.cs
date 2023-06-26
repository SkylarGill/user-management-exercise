using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAll().ConfigureAwait(false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users);
    } 
    
    [Fact]
    public async Task FilterByActive_WhenIsActiveParamIsTrue_MustOnlyReturnEntitiesTrueIsActiveValue()
    {
        // Arrange
        var service = CreateService();
        SetupUsers();

        // Act
        var result = await service.FilterByActive(true).ConfigureAwait(false);

        //Assert
        result.Should().AllSatisfy(model => model.IsActive.Should().BeTrue());;
    }
    
    [Fact]
    public async Task FilterByActive_WhenIsActiveParamIsFalse_MustOnlyReturnEntitiesWithFalseIsActiveValue()
    {
        // Arrange
        var service = CreateService();
        SetupUsers();

        // Act
        var result = await service.FilterByActive(false).ConfigureAwait(false);

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

        var mockDbSet = users.BuildMock(); 
        
        
        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(mockDbSet);

        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private readonly Mock<IAuditLogService> _auditLogService = new();
    private UserService CreateService() => new(_dataContext.Object, _auditLogService.Object);
}
