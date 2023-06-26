using System;
using System.Linq;
using UserManagement.Data.Entities;
using UserManagement.Models.AuditLogging;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Tests.Controllers.AuditLogsController;

public static class LogsControllerTestHelpers
{
    public const long NonExistentId = 999;

    public static Web.Controllers.AuditLogsController CreateController(Mock<IAuditLogService> auditLogService) =>
        new(auditLogService.Object);

    public static AuditLogEntry[] SetupAuditLogEntries(Mock<IAuditLogService> auditLogService)
    {
        var auditLogEntries = new AuditLogEntry[]
        {
            new()
            {
                Id = 1,
                Action = AuditLogAction.Create,
                Message = "Message 1",
                Time = new DateTime(2023, 06, 25, 10, 22, 10),
                UserId = 1,
                AfterSnapshotId = 1
            },
            new()
            {
                Id = 2,
                Action = AuditLogAction.Create,
                Message = "Message 2",
                Time = new DateTime(2023, 06, 25, 10, 24, 26),
                UserId = 2,
                AfterSnapshotId = 2
            },
            new()
            {
                Id = 4,
                Action = AuditLogAction.Update,
                Message = "Message 4",
                Time = new DateTime(2023, 06, 25, 10, 29, 25),
                UserId = 2,
                BeforeSnapshotId = 2,
                AfterSnapshotId = 3
            },
            new()
            {
                Id = 5,
                Action = AuditLogAction.Delete,
                Message = "Message 5",
                Time = new DateTime(2023, 06, 25, 11, 50, 25),
                UserId = 1,
                BeforeSnapshotId = 1,
            },
        };

        auditLogService.Setup(s => s.GetAll()).ReturnsAsync(auditLogEntries);

        var firstAuditLogEntry = auditLogEntries.First();
        auditLogService.Setup(s => s.GetAuditLogEntryById(firstAuditLogEntry.Id)).ReturnsAsync(firstAuditLogEntry);
        auditLogService.Setup(s => s.GetAuditLogEntryById(NonExistentId)).ReturnsAsync(null as AuditLogEntry);

        auditLogService
            .Setup(s => s.FilterByAction(AuditLogAction.Create))
            .ReturnsAsync(auditLogEntries.Where(entry => entry.Action == AuditLogAction.Create));
        auditLogService
            .Setup(s => s.FilterByAction(AuditLogAction.Update))
            .ReturnsAsync(auditLogEntries.Where(entry => entry.Action == AuditLogAction.Update));
        auditLogService
            .Setup(s => s.FilterByAction(AuditLogAction.Delete))
            .ReturnsAsync(auditLogEntries.Where(entry => entry.Action == AuditLogAction.Delete));

        return auditLogEntries;
    }
}