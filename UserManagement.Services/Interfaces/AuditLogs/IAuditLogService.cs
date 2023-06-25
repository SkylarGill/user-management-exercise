using System.Collections.Generic;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces.AuditLogs;

public interface IAuditLogService
{
    IEnumerable<AuditLogEntry> GetAll();
    AuditLogEntry? GetAuditLogEntryById(long id);
    void LogCreate(User user);
    void LogUpdate(User before, User after);
}