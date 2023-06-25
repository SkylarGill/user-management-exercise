using System.Linq;
using UserManagement.Models.Logging;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Controllers;

[Route("logs")]
public class AuditLogsController : Controller
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public ViewResult List()
    {
        var items = _auditLogService.GetAll()
            .Select(
                entry => new LogListItemViewModel
                {
                    Id = entry.Id,
                    Action = entry.Action,
                    Message = entry.Message,
                    Time = entry.Time,
                    UserId = entry.UserId,
                });

        var model = new LogListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("{id:long}")]
    public IActionResult Details([FromRoute] long id)
    {
        var auditLogEntry = _auditLogService.GetAuditLogEntryById(id);

        if (auditLogEntry is null)
        {
            return NotFound();
        }
        
        var beforeSnapshot = auditLogEntry.BeforeSnapshot is null
            ? null
            : new LogSnapshotViewModel()
            {
                Title = "Before",
                Forename = auditLogEntry.BeforeSnapshot.Forename,
                Surname = auditLogEntry.BeforeSnapshot.Surname,
                Email = auditLogEntry.BeforeSnapshot.Email,
                DateOfBirth = auditLogEntry.BeforeSnapshot.DateOfBirth,
                IsActive = auditLogEntry.BeforeSnapshot.IsActive,
            };
        
        var afterSnapshot = auditLogEntry.AfterSnapshot is null
            ? null
            : new LogSnapshotViewModel
            {
                Title = "After",
                Forename = auditLogEntry.AfterSnapshot.Forename,
                Surname = auditLogEntry.AfterSnapshot.Surname,
                Email = auditLogEntry.AfterSnapshot.Email,
                DateOfBirth = auditLogEntry.AfterSnapshot.DateOfBirth,
                IsActive = auditLogEntry.AfterSnapshot.IsActive,
            };

        var viewModel = new LogDetailsViewModel()
        {
            Id = auditLogEntry.Id,
            Action = auditLogEntry.Action,
            Message = auditLogEntry.Message,
            Time = auditLogEntry.Time,
            UserId = auditLogEntry.UserId,
            BeforeSnapshot = beforeSnapshot,
            AfterSnapshot = afterSnapshot,
        };

        return View(viewModel);
    }
}