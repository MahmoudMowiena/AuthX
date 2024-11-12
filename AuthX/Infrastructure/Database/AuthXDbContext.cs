using AuthX.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Infrastructure.Database;

public class AuthXDbContext : IdentityDbContext<User, Role, string>
{
    // public DbSet<Session> Sessions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public AuthXDbContext() { }
    public AuthXDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // modelBuilder.Entity<User>()
        //     .HasMany(u => u.Roles)
        //     .WithMany(r => r.Users)
        //     .UsingEntity(j => j.ToTable("UserRoles"));

         modelBuilder.Entity<Role>()
        .HasMany(r => r.Permissions)
        .WithMany(p => p.Roles)
        .UsingEntity<Dictionary<string, object>>(
            "RolePermission",  // The junction table
            j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
            j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId")
        );

    modelBuilder.Entity<Role>().HasData(
        new Role
        {
            Id = "1",
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN"
        });

    // Seed users next
    modelBuilder.Entity<User>().HasData(
        new User
        {
            Id = "1",
            PhoneNumber = "01100200300",
            UserName = "user1@example.com",
            NormalizedUserName = "USER1@EXAMPLE.COM",
            Email = "user1@example.com",
            NormalizedEmail = "USER1@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123!") // Hash the password
        });

    // Seed user-role relationship after roles and users have been seeded
    modelBuilder.Entity<IdentityUserRole<string>>().HasData(
        new IdentityUserRole<string>
        {
            UserId = "1",
            RoleId = "1"
        });
    }
}

// scalable approaches for detecting and resolving conflicts in a replicated system.
