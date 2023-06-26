namespace UserManagement.Models.AuditLogging;

public class LogNotFoundViewModel
{
    public LogNotFoundViewModel(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
