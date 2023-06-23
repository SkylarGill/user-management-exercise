using System;
using System.Linq;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet]
    public ViewResult List(FilterType filterType = FilterType.All)
    {
        var items = GetFilteredUsers(filterType)
            .Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive
            });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    private IEnumerable<User> GetFilteredUsers(FilterType filterType) =>
        filterType switch
        {
            FilterType.All => _userService.GetAll(),
            FilterType.Active => _userService.FilterByActive(true),
            FilterType.NonActive => _userService.FilterByActive(false),
            _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null)
        };
}

public enum FilterType
{
    All,
    Active,
    NonActive
}