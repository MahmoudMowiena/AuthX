using AuthX.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Infrastructure.Database;

public class AuthXDbContext : IdentityDbContext<UserModel, RoleModel, string>
{
    public DbSet<SessionModel> Sessions { get; set; }
    public DbSet<PermissionModel> Permissions { get; set; }
    public DbSet<AuditLogModel> AuditLogs { get; set; }

    public AuthXDbContext() { }
    public AuthXDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("UserRoles"));

        modelBuilder.Entity<RoleModel>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity(j => j.ToTable("RolePermissions"));
    }
}