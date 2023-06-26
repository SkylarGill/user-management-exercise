namespace UserManagement.Models.AuditLogging;

public class LogEntryNotFoundViewModel
{
    public LogEntryNotFoundViewModel(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
