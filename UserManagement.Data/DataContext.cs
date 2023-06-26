using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Models.AuditLogging;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    protected override void OnModelCreating(ModelBuilder model)
    {
        var users = new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", IsActive = true, DateOfBirth = new DateTime(1990, 6, 24)},
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", IsActive = true, DateOfBirth = new DateTime(1960, 4, 4) },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", IsActive = false, DateOfBirth = new DateTime(1986, 2, 15) },
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", IsActive = true, DateOfBirth = new DateTime(1997, 11, 21) },
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", IsActive = true, DateOfBirth = new DateTime(1973, 6, 15) },
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", IsActive = true, DateOfBirth = new DateTime(1978, 10, 17) },
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", IsActive = false, DateOfBirth = new DateTime(1995, 3, 23) },
            new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", IsActive = false, DateOfBirth = new DateTime(1955, 11, 5) },
            new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", IsActive = false, DateOfBirth = new DateTime(1963, 12, 25) },
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", IsActive = true, DateOfBirth = new DateTime(1994, 5, 21) },
            new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", IsActive = true, DateOfBirth = new DateTime(1985, 10, 25) },
        };
        
        model.Entity<User>().HasData(users);

        var auditLogEntries = users.Select(
            (u, i) => new AuditLogEntry
            {
                Id = i + 1,
                Time = DateTime.Now,
                Action = AuditLogAction.Create,
                AfterSnapshotId = i + 1,
                UserId = u.Id,
                Message = $"User created with ID '{u.Id}'"
            });

        var auditLogSnapshots = users.Select((u, i) => new AuditLogSnapshot(u, i + 1));

        model.Entity<AuditLogSnapshot>()
            .HasData(auditLogSnapshots);
        model.Entity<AuditLogEntry>()
            .HasData(auditLogEntries);
    }

    public DbSet<User>? Users { get; set; }
    public DbSet<AuditLogEntry>? AuditLogEntries { get; set; }
    public DbSet<AuditLogSnapshot>? AuditLogSnapshots { get; set; }

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        => base.Set<TEntity>();

    public async Task Create<TEntity>(TEntity entity) where TEntity : class
    {
        await base.AddAsync(entity).ConfigureAwait(false);
        await SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        await SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task Delete<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        await SaveChangesAsync().ConfigureAwait(false);
    }
}