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
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupUser> GroupUsers { get; set; }
    public DbSet<GroupMenu> GroupMenus { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SystemNotification> SystemNotifications { get; set; }
    
    // Future-ready DbSets (comment out for now)
    // public DbSet<GroupDepartment> GroupDepartments { get; set; }
    // public DbSet<GroupCategory> GroupCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.RoleId);
            
            // Configure Department relationship
            entity.HasOne(u => u.Department)
                  .WithMany(d => d.Users)
                  .HasForeignKey(u => u.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Group entity
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure GroupUser entity (Many-to-Many)
        modelBuilder.Entity<GroupUser>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GroupId });
            
            entity.HasOne(gu => gu.User)
                  .WithMany(u => u.GroupUsers)
                  .HasForeignKey(gu => gu.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(gu => gu.Group)
                  .WithMany(g => g.GroupUsers)
                  .HasForeignKey(gu => gu.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GroupMenu entity (Many-to-Many)
        modelBuilder.Entity<GroupMenu>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.MenuId });
            
            entity.HasOne(gm => gm.Group)
                  .WithMany(g => g.GroupMenus)
                  .HasForeignKey(gm => gm.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(gm => gm.Menu)
                  .WithMany(m => m.GroupMenus)
                  .HasForeignKey(gm => gm.MenuId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.MapId).IsUnique().HasFilter("\"MAP_ID\" IS NOT NULL");
            
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
            entity.HasIndex(e => e.MapId).IsUnique().HasFilter("\"MAP_ID\" IS NOT NULL");
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
