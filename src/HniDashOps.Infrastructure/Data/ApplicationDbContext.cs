using Microsoft.EntityFrameworkCore;
using HniDashOps.Core.Entities;

namespace HniDashOps.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SystemNotification> SystemNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Configure Department relationship
            entity.HasOne(u => u.Department)
                  .WithMany(d => d.Users)
                  .HasForeignKey(u => u.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Permission entity
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure UserRole entity (Many-to-Many)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        });

        // Configure RolePermission entity (Many-to-Many)
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.MapId).IsUnique().HasFilter("\"MAPID\" IS NOT NULL");
            
            // Configure self-referencing relationship (Parent-Child)
            entity.HasOne(d => d.Parent)
                  .WithMany(d => d.Children)
                  .HasForeignKey(d => d.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Menu entity
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasIndex(e => new { e.ParentId, e.Order });
            
            // Configure self-referencing relationship (Parent-Child)
            entity.HasOne(m => m.Parent)
                  .WithMany(m => m.Children)
                  .HasForeignKey(m => m.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.MapId).IsUnique().HasFilter("\"MAPID\" IS NOT NULL");
            entity.HasIndex(e => new { e.ParentId, e.Order });
            entity.HasIndex(e => e.Type);
            
            // Configure self-referencing relationship (Parent-Child)
            entity.HasOne(c => c.Parent)
                  .WithMany(c => c.Children)
                  .HasForeignKey(c => c.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure SystemNotification entity
        modelBuilder.Entity<SystemNotification>(entity =>
        {
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.TargetAudience);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.ReadBy);
            entity.HasIndex(e => new { e.StartAt, e.EndAt });
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
