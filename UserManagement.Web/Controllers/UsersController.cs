using System;
using System.Linq;
using FluentValidation;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Models.AuditLogging;
using UserManagement.Models.Users;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserViewModel> _createUserViewModelValidator;
    private readonly IValidator<EditUserViewModel> _editUserViewModelValidator;
    private readonly IAuditLogService _auditLogService;

    public UsersController(
        IUserService userService, 
        IValidator<CreateUserViewModel> createUserViewModelValidator, 
        IValidator<EditUserViewModel> editUserViewModelValidator,
        IAuditLogService auditLogService)
    {
        _userService = userService;
        _createUserViewModelValidator = createUserViewModelValidator;
        _editUserViewModelValidator = editUserViewModelValidator;
        _auditLogService = auditLogService;
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

    [HttpGet]
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

    [HttpGet]
    [Route("{id:long}")]
    public IActionResult Details ([FromRoute]long id)
    {
        var user = _userService.GetUserById(id);

        if (user is null)
        {
            return RedirectToAction("UserNotFound", new { id = id, });
        }

        var auditLogEntries = _auditLogService.GetAll()
            .Where(entry => entry.UserId == id)
            .Select(entry => new LogListItemViewModel
            {
                Id = entry.Id,
                Action = entry.Action,
                Message = entry.Message,
                Time = entry.Time,
                UserId = entry.UserId,
            });

        var userViewModel = new UserDetailsViewModel()
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            AuditLogEntries = auditLogEntries.ToList()
        };
        
        return View(userViewModel);
    }

    [HttpGet]
    [Route("edit/{id:long}")]
    public IActionResult Edit([FromRoute] long id, EditUserViewModel editUserViewModel)
    {
        if (editUserViewModel.HasValidationErrors)
        {
            return View(editUserViewModel);
        }
        
        var user = _userService.GetUserById(id);

        if (user is null)
        {
            return RedirectToAction("UserNotFound", new { id = id, });
        }

        var userViewModel = new EditUserViewModel()
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive
        };

        ModelState.Clear();
        return View(userViewModel);
    }

    [HttpPost]
    [Route("edit/{id:long}")]
    public IActionResult SubmitEdit(long id, EditUserViewModel editUserViewModel)
    {
        var validationResult = _editUserViewModelValidator.Validate(editUserViewModel);

        if (!validationResult.IsValid)
        {
            ModelState.Clear();
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);   
            }   
            
            editUserViewModel.HasValidationErrors = true;
            return View("Edit", editUserViewModel);
        }

        var user = _userService.GetUserById(editUserViewModel.Id);
        if (user is null)
        {
            return RedirectToAction("UserNotFound", new { id = id });
        }

        user.Forename = editUserViewModel.Forename;
        user.Surname = editUserViewModel.Surname;
        user.Email = editUserViewModel.Email;
        user.IsActive = editUserViewModel.IsActive;
        user.DateOfBirth = editUserViewModel.DateOfBirth;

        _userService.UpdateUser(user);

        return RedirectToAction("List");
    }

    
    [HttpGet]
    [Route("delete/{id:long}")]
    public IActionResult Delete(long id)
    {
        var user = _userService.GetUserById(id);

        if (user is null)
        {
            return RedirectToAction("UserNotFound", new { id = id });
        }
        
        _userService.DeleteUser(user);

        return RedirectToAction("List");
    }
    

    [HttpGet]
    [Route("notfound/{id:long}")]
    public IActionResult UserNotFound([FromRoute] long id)
    {
        return View(new UserNotFoundViewModel(id));
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

