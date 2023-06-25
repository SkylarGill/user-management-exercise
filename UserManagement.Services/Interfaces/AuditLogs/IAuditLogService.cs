using System.Collections.Generic;
using UserManagement.Data.Entities;
using UserManagement.Models.Logging;

namespace UserManagement.Services.Interfaces.AuditLogs;

public interface IAuditLogService
{
    IEnumerable<AuditLogEntry> GetAll();
    IEnumerable<AuditLogEntry> FilterByAction(AuditLogAction filterType);
    AuditLogEntry? GetAuditLogEntryById(long id);
    void LogCreate(User user);
    void LogUpdate(User before, User after);
    void LogDelete(User user);
}