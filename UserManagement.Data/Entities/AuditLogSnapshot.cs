using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Data.Entities;

public class AuditLogSnapshot
{
    public AuditLogSnapshot() { }

    public AuditLogSnapshot(User user)
    {
        Forename = user.Forename;
        Surname = user.Surname;
        Email = user.Email;
        DateOfBirth = user.DateOfBirth;
        IsActive = user.IsActive;
    }
    
    public AuditLogSnapshot(User user, long id)
    {
        Id = id;
        Forename = user.Forename;
        Surname = user.Surname;
        Email = user.Email;
        DateOfBirth = user.DateOfBirth;
        IsActive = user.IsActive;
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required]
    public string Forename { get; set; } = default!;
    [Required]
    public string Surname { get; set; } = default!;
    [Required] 
    [EmailAddress]
    public string Email { get; set; } = default!;
    public DateTime DateOfBirth { get; set; } = default!;
    public bool IsActive { get; set; }
}