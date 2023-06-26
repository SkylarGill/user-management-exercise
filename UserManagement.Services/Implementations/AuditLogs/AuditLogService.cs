using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Models.AuditLogging;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Services.Implementations.AuditLogs;

public class AuditLogService : IAuditLogService
{
    private readonly IDataContext _dataContext;
    private readonly ICurrentDateProvider _currentDateProvider;

    public AuditLogService(IDataContext dataContext, ICurrentDateProvider currentDateProvider)
    {
        _dataContext = dataContext;
        _currentDateProvider = currentDateProvider;
    }

    public async Task<IEnumerable<AuditLogEntry>> GetAll() => 
        await _dataContext
            .GetAll<AuditLogEntry>()
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<IEnumerable<AuditLogEntry>> FilterByAction(AuditLogAction filterType) =>
        await _dataContext
            .GetAll<AuditLogEntry>()
            .Where(entry => entry.Action == filterType)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<IEnumerable<AuditLogEntry>> FilterByUserId(long userId) =>
        await _dataContext
            .GetAll<AuditLogEntry>()
            .Where(entry => entry.UserId == userId)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<AuditLogEntry?> GetAuditLogEntryById(long id) =>
        await _dataContext
            .GetAll<AuditLogEntry>()
            .Include(entry => entry.AfterSnapshot)
            .Include(entry => entry.BeforeSnapshot)
            .FirstOrDefaultAsync(entry => entry.Id == id)
            .ConfigureAwait(false);

    public async Task LogCreate(User user)
    {
        var logEntry = new AuditLogEntry
        {
            Action = AuditLogAction.Create,
            AfterSnapshot = new AuditLogSnapshot(user),
            Time = _currentDateProvider.GetCurrentDateTime(),
            Message = $"User created with ID '{user.Id}'",
            UserId = user.Id,
        };

        await _dataContext.Create(logEntry).ConfigureAwait(false);
    }

    public void LogUpdate(User before, User after)
    {
        var logEntry = new AuditLogEntry
        {
            Action = AuditLogAction.Update,
            BeforeSnapshot = new AuditLogSnapshot(before),
            AfterSnapshot = new AuditLogSnapshot(after),
            Time = _currentDateProvider.GetCurrentDateTime(),
            Message = $"User updated with ID '{after.Id}'",
            UserId = after.Id,
        };

        _dataContext.Create(logEntry);
    }

    public void LogDelete(User user)
    {
        var logEntry = new AuditLogEntry
        {
            Action = AuditLogAction.Delete,
            BeforeSnapshot = new AuditLogSnapshot(user),
            Time = _currentDateProvider.GetCurrentDateTime(),
            Message = $"User deleted with ID '{user.Id}'",
            UserId = user.Id,
        };

        _dataContext.Create(logEntry);
    }
}