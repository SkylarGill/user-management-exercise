using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.AuditLogging;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.AuditLogsController;

public class LogsControllerDetailsTests
{
    private readonly Mock<IAuditLogService> _auditLogService = new();

    [Fact]
    public async Task Details_WhenRequestingExistingLogEntryId_ReturnsViewModelWithCorrectParameters()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
        var auditLogEntry = LogsControllerTestHelpers.SetupAuditLogEntries(_auditLogService).First();
        var auditLogEntryAsViewModel = new LogListItemViewModel
        {
            Action = auditLogEntry.Action,
            Id = auditLogEntry.Id,
            Message = auditLogEntry.Message,
            Time = auditLogEntry.Time,
            UserId = auditLogEntry.UserId
        };
        
        // Act
        var result = await controller.Details(auditLogEntry.Id).ConfigureAwait(false);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<LogDetailsViewModel>()
            .And.BeEquivalentTo(auditLogEntryAsViewModel);
    }

    [Fact]
    public async Task Details_WhenRequestingNonExistingLogEntryId_ReturnsRedirectToActionOfLogEntryNotFound()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
        LogsControllerTestHelpers.SetupAuditLogEntries(_auditLogService);

        // Act
        var result = await controller.Details(LogsControllerTestHelpers.NonExistentId).ConfigureAwait(false);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().BeEquivalentTo("LogEntryNotFound");

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult?.RouteValues
            .Should().HaveCount(1).And.ContainKey("id")
            .WhoseValue.Should().BeEquivalentTo(LogsControllerTestHelpers.NonExistentId);
    }
}