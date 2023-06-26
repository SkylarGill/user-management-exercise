using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models.AuditLogging;
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
    public async Task<ViewResult> List(AuditLogActionFilterType filterType = AuditLogActionFilterType.All)
    {
        var auditLogEntries = await GetFilteredAuditLogs(filterType).ConfigureAwait(false);

        var items = auditLogEntries
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
    public async Task<IActionResult> Details([FromRoute] long id)
    {
        var auditLogEntry = await _auditLogService.GetAuditLogEntryById(id).ConfigureAwait(false);

        if (auditLogEntry is null)
        {
            return RedirectToAction("LogEntryNotFound", new { id = id });
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
    
    [HttpGet]
    [Route("notfound/{id:long}")]
    public IActionResult LogEntryNotFound([FromRoute] long id)
    {
        return View(new LogEntryNotFoundViewModel(id));
    }

    private async Task<IEnumerable<AuditLogEntry>> GetFilteredAuditLogs(AuditLogActionFilterType filterType) =>
        filterType switch
        {
            AuditLogActionFilterType.All => await _auditLogService.GetAll().ConfigureAwait(false),
            AuditLogActionFilterType.Create => await _auditLogService.FilterByAction(AuditLogAction.Create).ConfigureAwait(false),
            AuditLogActionFilterType.Update => await _auditLogService.FilterByAction(AuditLogAction.Update).ConfigureAwait(false),
            AuditLogActionFilterType.Delete => await _auditLogService.FilterByAction(AuditLogAction.Delete).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null)
        };
}