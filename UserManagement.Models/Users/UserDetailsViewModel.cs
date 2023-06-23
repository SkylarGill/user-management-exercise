using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Models.Users;

[BindProperties]
public class UserDetailsViewModel
{
    public long Id { get; set; }
    
    public string Forename { get; set; } = default!;

    public string Surname { get; set; } = default!;
    
    public string Email { get; set; } = default!;

    public DateTime DateOfBirth { get; set; } = default!;

    public bool IsActive { get; set; }
}