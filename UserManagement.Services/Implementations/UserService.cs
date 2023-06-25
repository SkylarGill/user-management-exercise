using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Data.Entities;
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
    public void CreateUser(User user)
    {
        _dataAccess.Create(user);
        _auditLogService.LogCreate(user);
    }
    
    public void UpdateUser(User user)
    {
        _dataAccess.Update(user);
    }

    public User? GetUserById (long id) =>
        _dataAccess.GetAll<User>()
            .FirstOrDefault(user => user.Id == id);

    public void DeleteUser(User user)
    {
        _dataAccess.Delete(user);
    }
}