using System;
using System.Linq;
using FluentValidation;
using UserManagement.Data.Entities;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserViewModel> _createUserViewModelValidator;

    public UsersController(IUserService userService, IValidator<CreateUserViewModel> createUserViewModelValidator)
    {
        _userService = userService;
        _createUserViewModelValidator = createUserViewModelValidator;
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
                IsActive = p.IsActive,
                DateOfBirth = p.DateOfBirth
            });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [Route("create")]
    public ViewResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [Route("create")]
    public IActionResult Create([Bind] CreateUserViewModel createUserViewModel)
    {
        var validationResult = _createUserViewModelValidator.Validate(createUserViewModel);

        if (!validationResult.IsValid)
        {
            ModelState.Clear();
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);   
            }   
        }

        if (!ModelState.IsValid)
        {
            return View(createUserViewModel);
        }

        var user = new User()
        {
            Forename = createUserViewModel.Forename,
            Surname = createUserViewModel.Surname,
            Email = createUserViewModel.Email,
            IsActive = createUserViewModel.IsActive,
            DateOfBirth = createUserViewModel.DateOfBirth
        };
        
        _userService.CreateUser(user);

        return RedirectToAction("List");
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