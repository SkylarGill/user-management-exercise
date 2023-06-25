using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

    public AuditLogEntry? GetAuditLogEntryById(long id) =>
        _dataContext
            .GetAll<AuditLogEntry>()
            .Include(entry => entry.AfterSnapshot)
            .Include(entry => entry.BeforeSnapshot)
            .FirstOrDefault(entry => entry.Id == id)
        ;
}