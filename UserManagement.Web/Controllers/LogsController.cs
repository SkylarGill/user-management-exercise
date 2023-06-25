using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.Logs;
using UserManagement.Services.Implementations.AuditLogs;
using UserManagement.Services.Interfaces.AuditLogs;

namespace UserManagement.Web.Controllers;

[Route("logs")]
public class LogsController : Controller
{
    private readonly IAuditLogService _auditLogService;

    public LogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }
    
    [HttpGet]
    public ViewResult List()
    {
        var items = _auditLogService.GetAll()
            .Select(entry => new LogListItemViewModel
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
}