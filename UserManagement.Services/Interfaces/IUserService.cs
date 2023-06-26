using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    IEnumerable<User> FilterByActive(bool isActive);
    IEnumerable<User> GetAll();
    Task CreateUser(User user);
    Task UpdateUser(User user);
    User? GetUserById(long id);
    void DeleteUser(User user);
}
