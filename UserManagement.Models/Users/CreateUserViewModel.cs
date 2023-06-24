namespace UserManagement.Models.Users;

public class CreateUserViewModel
{
    public string Forename { get; set; } = default!;

    public string Surname { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DateTime DateOfBirth { get; set; } = default!;

    public bool IsActive { get; set; }
}