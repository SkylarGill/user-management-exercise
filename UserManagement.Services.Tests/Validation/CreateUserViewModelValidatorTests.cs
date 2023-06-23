using System;
using UserManagement.Models.Users;
using UserManagement.Services.Implementations.Validation;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Tests.Validation;

public class CreateUserViewModelValidatorTests
{
    private readonly DateTime _currentDate = new DateTime(2023, 06, 23);
    
    [Fact]
    public void Validate_WhenAllPropertiesAreValid_MustReturnNoValidationErrors()
    {
        // Arrange
        var service = CreateService();
        var model = GetValidModel();
        
        // Act
        var result = service.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WhenForenameIsInvalid_MustReturnValidationError(string invalidValue)
    {
        // Arrange
        var service = CreateService();
        var model = GetValidModel();
        model.Forename = invalidValue;
        
        // Act
        var result = service.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should()
            .ContainSingle(failure => failure.ErrorMessage == "Forename must not be empty or whitespace");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WhenSurnameIsInvalid_MustReturnValidationError(string invalidValue)
    {
        // Arrange
        var service = CreateService();
        var model = GetValidModel();
        model.Surname = invalidValue;
        
        // Act
        var result = service.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should()
            .ContainSingle(failure => failure.ErrorMessage == "Surname must not be empty or whitespace");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("example")]
    [InlineData("@example.com")]
    [InlineData("example@")]
    public void Validate_WhenEmailIsInvalid_MustReturnValidationError(string invalidValue)
    {
        // Arrange
        var service = CreateService();
        var model = GetValidModel();
        model.Email = invalidValue;
        
        // Act
        var result = service.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should()
            .ContainSingle(failure => failure.ErrorMessage == "Email must be a valid email address");
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(365)]
    [InlineData(3650)]
    public void Validate_WhenDateOfBirthIsInFuture_MustReturnValidationError(double daysToAdd)
    {
        // Arrange
        var service = CreateService();
        var model = GetValidModel();
        model.DateOfBirth = _currentDate.AddDays(daysToAdd);
        
        // Act
        var result = service.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should()
            .ContainSingle(failure => failure.ErrorMessage == "Date of Birth cannot be in the future");
    }
    
    [Fact]
    public void Validate_WhenDateOfBirthIsDefaultDateTime_MustReturnValidationError()
    {
        // Arrange
        var service = CreateService();
        var model = GetValidModel();
        model.DateOfBirth = default!;
        
        // Act
        var result = service.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should()
            .ContainSingle(failure => failure.ErrorMessage == "Date of Birth must be specified");
    }

    private static CreateUserViewModel GetValidModel() =>
        new()
        {
            Forename = "David",
            Surname = "NonActive",
            Email = "inactive@example.com",
            IsActive = false,
            DateOfBirth = new DateTime(1984, 12, 1)
        };

    private CreateUserViewModelValidator CreateService()
    {
        var currentDateProvider = new Mock<ICurrentDateProvider>();
        currentDateProvider.Setup(provider => provider.GetCurrentDate()).Returns(_currentDate);

        var service = new CreateUserViewModelValidator(currentDateProvider.Object);
        return service;
    }
}