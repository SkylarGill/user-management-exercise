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
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
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
        var result = await controller.List().ConfigureAwait(false);

        // Assert
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().BeEquivalentTo(auditLogEntriesAsViewModels);
    }
    
    [Fact]
    public async Task List_WhenSpecifyingActionFilterOfCreate_ModelMustOnlyContainAuditLogsWithCreateAction()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
        LogsControllerTestHelpers.SetupAuditLogEntries(_auditLogService);

        // Act
        var result = await controller.List(AuditLogActionFilterType.Create).ConfigureAwait(false);

        // Assert
        _auditLogService.Verify(service => service.FilterByAction(AuditLogAction.Create));
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.Action.Should().Be(AuditLogAction.Create));
    }
    
    [Fact]
    public async Task List_WhenSpecifyingActionFilterOfUpdate_ModelMustOnlyContainAuditLogsWithUpdateAction()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
        LogsControllerTestHelpers.SetupAuditLogEntries(_auditLogService);

        // Act
        var result = await controller.List(AuditLogActionFilterType.Update).ConfigureAwait(false);

        // Assert
        _auditLogService.Verify(service => service.FilterByAction(AuditLogAction.Update));
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.Action.Should().Be(AuditLogAction.Update));
    }
    
    [Fact]
    public async Task List_WhenSpecifyingActionFilterOfDelete_ModelMustOnlyContainAuditLogsWithDeleteAction()
    {
        // Arrange
        var controller = LogsControllerTestHelpers.CreateController(_auditLogService);
        LogsControllerTestHelpers.SetupAuditLogEntries(_auditLogService);

        // Act
        var result = await controller.List(AuditLogActionFilterType.Delete).ConfigureAwait(false);

        // Assert
        _auditLogService.Verify(service => service.FilterByAction(AuditLogAction.Delete));
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().AllSatisfy(model => model.Action.Should().Be(AuditLogAction.Delete));
    }
    

}