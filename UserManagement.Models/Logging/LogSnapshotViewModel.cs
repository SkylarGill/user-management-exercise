namespace UserManagement.Models.Logging;

public class LogSnapshotViewModel
{
    public string Title { get; set; } = default!;
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime DateOfBirth { get; set; } = default!;
    public bool IsActive { get; set; }
}