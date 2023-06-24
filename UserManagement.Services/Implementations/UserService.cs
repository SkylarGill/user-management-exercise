﻿using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

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