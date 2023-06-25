namespace UserManagement.Models.Logging;

public class LogDetailsViewModel
{
    public long Id { get; set; }

    public DateTime Time { get; set; }

    public AuditLogAction Action { get; set; }

    public long UserId { get; set; }

    public LogSnapshotViewModel? BeforeSnapshot { get; set; }
    public LogSnapshotViewModel? AfterSnapshot { get; set; }

    public string? Message { get; set; }
}