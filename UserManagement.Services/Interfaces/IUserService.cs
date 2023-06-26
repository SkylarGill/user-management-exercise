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
    Task<IEnumerable<User>> FilterByActive(bool isActive);
    Task<IEnumerable<User>> GetAll();
    Task CreateUser(User user);
    Task UpdateUser(User user);
    Task<User?> GetUserById(long id);
    Task DeleteUser(User user);
}
