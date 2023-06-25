using System.Collections.Generic;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Services.Implementations.AuditLogs;

public class AuditLogService : IAuditLogService
{
    private readonly IDataContext _dataContext;

    public AuditLogService(IDataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public IEnumerable<AuditLogEntry> GetAll() => _dataContext.GetAll<AuditLogEntry>();
}