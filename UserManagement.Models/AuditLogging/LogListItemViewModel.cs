namespace UserManagement.Models.AuditLogging;

public class LogListItemViewModel
{
    public long Id { get; set; }

    public DateTime Time { get; set; }

    public AuditLogAction Action { get; set; }

    public long UserId { get; set; }

    public string? Message { get; set; }
}