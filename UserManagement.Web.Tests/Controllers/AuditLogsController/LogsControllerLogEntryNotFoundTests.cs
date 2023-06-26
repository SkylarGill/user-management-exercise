using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.AuditLogging;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.AuditLogsController;

public class LogsControllerLogEntryNotFoundTests
{
    private readonly Mock<IAuditLogService> _auditLogService = new();

    [Fact]
    public void Details_WhenRequestingExistingLogEntryId_ReturnsViewModelWithCorrectParameters()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
        var viewModel = new LogEntryNotFoundViewModel(LogsControllerTestHelpers.NonExistentId);
        
        // Act
        var result = controller.LogEntryNotFound(LogsControllerTestHelpers.NonExistentId);

        //Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<LogEntryNotFoundViewModel>()
            .And.BeEquivalentTo(viewModel);
    }
}