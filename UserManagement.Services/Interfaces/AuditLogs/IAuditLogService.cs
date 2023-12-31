using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models.AuditLogging;

namespace UserManagement.Services.Interfaces.AuditLogs;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogEntry>> GetAll();
    Task<IEnumerable<AuditLogEntry>> FilterByAction(AuditLogAction filterType);
    Task<IEnumerable<AuditLogEntry>> FilterByUserId(long userId);
    Task<AuditLogEntry?> GetAuditLogEntryById(long id);
    Task LogCreate(User user);
    Task LogUpdate(User before, User after);
    Task LogDelete(User user);
}