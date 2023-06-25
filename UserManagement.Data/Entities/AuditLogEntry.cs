using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagement.Models.Logs;

namespace UserManagement.Data.Entities;

public class AuditLogEntry
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public DateTime Time { get; set; }
    
    public AuditLogAction Action { get; set; }
    
    public long UserId { get; set; }
    
    public long BeforeSnapshotId { get; set; }
    public long AfterSnapshotId { get; set; }
    
    public string? Message { get; set; }
}