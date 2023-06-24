using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Controllers;

namespace UserManagement.Web.Tests.Controllers.UserController;

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
        // Arrange
        var controller = CreateController();
        SetupUsers();

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
        var controller = CreateController();
        SetupUsers();

        // Act
        var result = controller.List(FilterType.NonActive);

        // Assert
        _userService.Verify(service => service.FilterByActive(false), Times.Once);
        result.Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.IsActive.Should().BeFalse());
    }

    [Fact]
    public void Create_WhenGettingPage_ReturnsViewModel()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.Create();

        // Assert
        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeNull();
    }

    [Fact]
    public void Create_WhenSubmittingUserWithNoValidationErrors_CallsCreateUserAndRedirectsToListAction()
    {
        // Arrange
        var controller = CreateController();
        SetupValidation();
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
        var controller = CreateController();
        const string createValidationMessage = "CreateUserViewModel validation failure";
        SetupValidation(false, nameof(CreateUserViewModel.Email), createValidationMessage,
            "EditUserViewModel validation failure");
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
        var controller = CreateController();
        const string createValidationMessage = "CreateUserViewModel validation failure";
        const string propertyName = nameof(CreateUserViewModel.Email);
        SetupValidation(false, propertyName, createValidationMessage, "EditUserViewModel validation failure");
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

    [Fact]
    public void Details_WhenRequestingExistingUserId_ReturnsViewModelWithCorrectParameters()
    {
        // Arrange
        var controller = CreateController();
        var user = SetupUsers().First();

        // Act
        var result = controller.Details(user.Id);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserDetailsViewModel>()
            .And.BeEquivalentTo(user);
    }

    [Fact]
    public void Details_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = CreateController();
        SetupUsers();
        const long userId = 999;

        // Act
        var result = controller.Details(userId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("UserNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(userId);
    }

    [Fact]
    public void Edit_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = CreateController();
        SetupUsers();
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
        var controller = CreateController();
        SetupValidation();
        var user = SetupUsers().First();

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
        var controller = CreateController();
        SetupValidation();
        SetupUsers();
        
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


    [Fact]
    public void SubmitEdit_WhenRequestingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = CreateController();
        SetupValidation();
        SetupUsers();
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
        var controller = CreateController();
        const string createValidationMessage = "CreateUserViewModel validation failure";
        SetupValidation(false, nameof(CreateUserViewModel.Email), createValidationMessage,
            "EditUserViewModel validation failure");
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
        var controller = CreateController();
        const string editValidationMessage = "EditUserViewModel validation failure";
        const string propertyName = nameof(CreateUserViewModel.Email);
        SetupValidation(false, propertyName, "CreateUserViewModel validation failure", editValidationMessage);
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
        var controller = CreateController();
        SetupValidation();
        var user = SetupUsers().First();

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

    [Fact]
    public void Delete_WhenDeletingNonExistingUserId_ReturnsRedirectToActionOfUserNotFound()
    {
        // Arrange
        var controller = CreateController();
        SetupUsers();
        const long userId = 999;

        // Act
        var result = controller.Delete(userId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("UserNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(userId);
    }

    [Fact]
    public void Delete_WhenDeletingExistingUserId_ReturnsCallsDeleteAndReturnsRedirectToActionOfList()
    {
        // Arrange
        var controller = CreateController();
        var user = SetupUsers().First();

        // Act
        var result = controller.Delete(user.Id);

        // Assert
        _userService.Verify(service => service.DeleteUser(user), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("List");
    }
    
    [Fact]
    public void UserNotFound_WhenRequestingWithUserId_ReturnsViewResultWithIdInModel()
    {
        // Arrange
        var controller = CreateController();
        var viewModel = new UserNotFoundViewModel(99);

        // Act
        var result = controller.UserNotFound(viewModel.Id);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserNotFoundViewModel>()
            .And.BeEquivalentTo(viewModel);
    }

    private void SetupValidation(bool shouldPassValidation = true, string propertyName = "",
        string createValidationMessage = "", string editValidationMessage = "")
    {
        _createUserViewModelValidator
            .Setup(
                validator => validator.Validate(It.IsAny<CreateUserViewModel>()))
            .Returns(new ValidationResult(
                shouldPassValidation
                    ? new List<ValidationFailure>()
                    : new List<ValidationFailure> { new(propertyName, createValidationMessage) }));
    
        _editUserViewModelValidator
            .Setup(
                validator => validator.Validate(It.IsAny<EditUserViewModel>()))
            .Returns(new ValidationResult(
                shouldPassValidation
                    ? new List<ValidationFailure>()
                    : new List<ValidationFailure> { new(propertyName, editValidationMessage) }));
    }
    
    private User[] SetupUsers()
    {
        var users = new[]
        {
            new User
            {
                Id = 1,
                Forename = "Johnny",
                Surname = "User",
                Email = "juser@example.com",
                IsActive = true,
                DateOfBirth = new DateTime(1990, 6, 24)
            },
            new User
            {
                Id = 2,
                Forename = "David",
                Surname = "NonActive",
                Email = "inactive@example.com",
                IsActive = false,
                DateOfBirth = new DateTime(1984, 12, 1)
            },
            new User
            {
                Id = 3,
                Forename = "Sarah",
                Surname = "Active",
                Email = "active@example.com",
                IsActive = true,
                DateOfBirth = new DateTime(1963, 7, 14)
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
    
        var firstUser = users.First();
    
        _userService
            .Setup(s => s.GetUserById(firstUser.Id)).Returns(firstUser);
        _userService
            .Setup(s => s.GetUserById(999)).Returns(null as User);
    
        return users;
    }
    
    private UsersController CreateController() =>
        new(_userService.Object,
            _createUserViewModelValidator.Object,
            _editUserViewModelValidator.Object);
    
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IValidator<CreateUserViewModel>> _createUserViewModelValidator = new();
    private readonly Mock<IValidator<EditUserViewModel>> _editUserViewModelValidator = new();

    
}