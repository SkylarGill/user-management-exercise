using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.AuditLogging;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.AuditLogsController;

public class LogsControllerListTests
{
    private readonly Mock<IAuditLogService> _auditLogService = new();
    
    [Fact]
    public async Task List_WhenServiceReturnsAuditLogEntries_ModelMustContainAuditLogs()
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
        var result = await controller.List();

        // Assert
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().BeEquivalentTo(auditLogEntriesAsViewModels);
    }

}