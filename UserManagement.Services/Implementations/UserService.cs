using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Exceptions;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Services.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    private readonly IAuditLogService _auditLogService;

    public UserService(IDataContext dataAccess, IAuditLogService auditLogService)
    {
        _dataAccess = dataAccess;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive) =>
        _dataAccess.GetAll<User>()
            .Where(user => user.IsActive == isActive);

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();
    public async Task CreateUser(User user)
    {
        await _dataAccess.Create(user).ConfigureAwait(false);
        await _auditLogService.LogCreate(user).ConfigureAwait(false);
    }
    
    public async Task UpdateUser(User user)
    {
        var oldUser = await _dataAccess
            .GetAll<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == user.Id)
            .ConfigureAwait(false);
        
        if (oldUser is null)
        {
            throw new UserMissingFromDataContextException(user.Id);
        }

        await _dataAccess.UpdateAsync(user).ConfigureAwait(false);
        await _auditLogService.LogUpdate(oldUser, user).ConfigureAwait(false);
    }

    public User? GetUserById (long id) =>
        _dataAccess.GetAll<User>()
            .FirstOrDefault(user => user.Id == id);

    public void DeleteUser(User user)
    {
        _dataAccess.Delete(user);
        _auditLogService.LogDelete(user);
    }
}