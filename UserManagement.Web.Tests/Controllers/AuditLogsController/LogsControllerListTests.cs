using System.Linq;
using UserManagement.Models.AuditLogging;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.LogsController;

public class LogsControllerListTests
{
    private readonly Mock<IAuditLogService> _auditLogService = new();
    
    [Fact]
    public void List_WhenServiceReturnsAuditLogEntries_ModelMustContainAuditLogs()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(
            _auditLogService);
        var auditLogEntries = LogsControllerTestHelpers.SetupAuditLogEntries(_auditLogService);
        var auditLogEntriesAsViewModels = auditLogEntries.Select(
            entry => new LogListItemViewModel()
            {
                Action = entry.Action, 
                Id = entry.Id, 
                Message = entry.Message, 
                Time = entry.Time, 
                UserId = entry.UserId
            });

        // Act
        var result = controller.List();

        // Assert
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().BeEquivalentTo(auditLogEntriesAsViewModels);
    }

}