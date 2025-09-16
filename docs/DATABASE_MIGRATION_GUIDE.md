# 🗄️ Database Migration Guide - HniDashOps Backend

## 📋 Tổng Quan

Tài liệu này hướng dẫn chi tiết về việc migration database cho dự án HniDashOps Backend, bao gồm cả trường hợp có sẵn database và tạo mới database.

## 🎯 Migration Strategies

### **1. Database-First Approach (Recommended cho Existing Database)**
```yaml
✅ Reverse Engineer từ existing database
✅ Generate Entity models từ schema hiện tại
✅ Preserve existing data
✅ No downtime
✅ Safe approach
```

### **2. Code-First Approach (Recommended cho New Database)**
```yaml
✅ Developer controls schema through code
✅ Version control database changes
✅ Automated migration scripts
✅ Team collaboration
✅ Environment consistency
```

### **3. Hybrid Approach (Best Practice)**
```yaml
✅ Start with Database-First để generate models
✅ Switch to Code-First cho future changes
✅ Best of both worlds
```

---

## 🗄️ Database-First Migration (Existing Database)

### **Khi Nào Sử Dụng:**
- Database đã có sẵn với data
- Schema đã được thiết kế và tested
- Cần preserve existing data
- Không muốn risk downtime

### **Step 1: Install Entity Framework Tools**

```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

### **Step 2: Scaffold Models từ Existing Database**

```bash
# PostgreSQL Database
dotnet ef dbcontext scaffold "Host=localhost;Database=hniDashOps;Username=postgres;Password=password" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c ApplicationDbContext --force

# MySQL Database
dotnet ef dbcontext scaffold "Server=localhost;Database=hniDashOps;User=root;Password=password;" Pomelo.EntityFrameworkCore.MySql -o Models -c ApplicationDbContext --force

# SQL Server Database
dotnet ef dbcontext scaffold "Server=localhost;Database=hniDashOps;Trusted_Connection=true;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c ApplicationDbContext --force
```

### **Step 3: Generated Files Structure**

Sau khi scaffold, EF Core sẽ tạo:

```
src/HniDashOps.Infrastructure/
├── Models/
│   ├── User.cs
│   ├── Menu.cs
│   ├── Category.cs
│   ├── Department.cs
│   ├── SystemNotification.cs
│   └── ApplicationDbContext.cs
```

### **Step 4: Generated ApplicationDbContext**

```csharp
// src/HniDashOps.Infrastructure/Models/ApplicationDbContext.cs
public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Menu> Menus { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<SystemNotification> SystemNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core auto-generates configurations từ existing schema
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            // ... other configurations
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
```

### **Step 5: Customize Generated Models**

```csharp
// src/HniDashOps.Infrastructure/Models/User.cs
public partial class User
{
    // Existing properties from database
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Add custom properties (not in database)
    [NotMapped]
    public string DisplayName => $"{FullName} ({Username})";
    
    // Add navigation properties
    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public virtual ICollection<SystemNotification> Notifications { get; set; } = new List<SystemNotification>();
    
    // Add business logic methods
    public bool HasPermission(string permission)
    {
        return Role == "Admin" || Role == "Manager";
    }
    
    public bool IsActiveUser()
    {
        return IsActive && CreatedAt <= DateTime.UtcNow;
    }
}
```

### **Step 6: Test Database Connection**

```bash
# Test connection
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Run application to verify
dotnet run --project src/HniDashOps.API
```

---

## 🏗️ Code-First Migration (New Database)

### **Khi Nào Sử Dụng:**
- Tạo database mới từ đầu
- Developer muốn control schema hoàn toàn
- Team muốn version control database changes
- Cần automated deployment

### **Step 1: Create Entity Models**

```csharp
// src/HniDashOps.Core/Models/User.cs
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Menu> Menus { get; set; }
    public virtual ICollection<SystemNotification> Notifications { get; set; }
}

// src/HniDashOps.Core/Models/Menu.cs
public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Icon { get; set; }
    public int? ParentId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual Menu Parent { get; set; }
    public virtual ICollection<Menu> Children { get; set; }
}

// src/HniDashOps.Core/Models/Category.cs
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// src/HniDashOps.Core/Models/Department.cs
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// src/HniDashOps.Core/Models/SystemNotification.cs
public class SystemNotification
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; }
}
```

### **Step 2: Create DbContext**

```csharp
// src/HniDashOps.Infrastructure/Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<SystemNotification> SystemNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### **Step 3: Create Entity Configurations**

```csharp
// src/HniDashOps.Infrastructure/Data/Configurations/UserConfiguration.cs
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        // Indexes
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        // Relationships
        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// src/HniDashOps.Infrastructure/Data/Configurations/MenuConfiguration.cs
public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(m => m.Path)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(m => m.Icon)
            .HasMaxLength(50);
            
        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        // Self-referencing relationship
        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(m => m.ParentId);
        builder.HasIndex(m => m.OrderIndex);
    }
}
```

### **Step 4: Create Initial Migration**

```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# This creates:
# Migrations/20241201_InitialCreate.cs
# Migrations/ApplicationDbContextModelSnapshot.cs
```

### **Step 5: Apply Migration to Database**

```bash
# Apply migration to database
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# This creates tables in database:
# - Users
# - Menus
# - Categories
# - Departments
# - SystemNotifications
```

---

## 🔄 Hybrid Migration Strategy

### **Phase 1: Start with Database-First**

```bash
# 1. Scaffold from existing database
dotnet ef dbcontext scaffold "ConnectionString" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c ApplicationDbContext

# 2. Customize generated models
# 3. Test with existing data
```

### **Phase 2: Switch to Code-First**

```bash
# 1. Create baseline migration
dotnet ef migrations add InitialMigration --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# 2. Apply migration (will be empty since schema already exists)
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

### **Phase 3: Future Changes**

```bash
# 1. Modify entity models
# 2. Create new migration
dotnet ef migrations add AddUserProfilePicture --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# 3. Apply migration
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

---

## 🚀 Migration Commands Reference

### **Database-First Commands**

```bash
# Scaffold from existing database
dotnet ef dbcontext scaffold "ConnectionString" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c ApplicationDbContext --force

# Test connection
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Update scaffolded models (when database changes)
dotnet ef dbcontext scaffold "ConnectionString" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c ApplicationDbContext --force
```

### **Code-First Commands**

```bash
# Create migration
dotnet ef migrations add MigrationName --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Apply migration
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Remove last migration
dotnet ef migrations remove --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Generate SQL script
dotnet ef migrations script --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# List migrations
dotnet ef migrations list --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

### **Utility Commands**

```bash
# Check database connection
dotnet ef database update --dry-run --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Generate migration script for specific range
dotnet ef migrations script FromMigration ToMigration --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Update database to specific migration
dotnet ef database update TargetMigration --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

---

## 📊 Generated SQL Examples

### **PostgreSQL Schema (Code-First)**

```sql
-- Users Table
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(50) NOT NULL,
    "Email" VARCHAR(100) NOT NULL,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "FullName" VARCHAR(100) NOT NULL,
    "Role" VARCHAR(20) NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Menus Table
CREATE TABLE "Menus" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Path" VARCHAR(200) NOT NULL,
    "Icon" VARCHAR(50),
    "ParentId" INTEGER,
    "OrderIndex" INTEGER NOT NULL DEFAULT 0,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Categories Table
CREATE TABLE "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Departments Table
CREATE TABLE "Departments" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- System Notifications Table
CREATE TABLE "SystemNotifications" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Message" TEXT NOT NULL,
    "Type" VARCHAR(20) NOT NULL,
    "IsRead" BOOLEAN NOT NULL DEFAULT false,
    "UserId" INTEGER,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ReadAt" TIMESTAMP
);

-- Indexes
CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");
CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
CREATE INDEX "IX_Menus_ParentId" ON "Menus" ("ParentId");
CREATE INDEX "IX_Menus_OrderIndex" ON "Menus" ("OrderIndex");
CREATE INDEX "IX_SystemNotifications_UserId" ON "SystemNotifications" ("UserId");

-- Foreign Keys
ALTER TABLE "Menus" ADD CONSTRAINT "FK_Menus_Menus_ParentId" 
    FOREIGN KEY ("ParentId") REFERENCES "Menus" ("Id") ON DELETE RESTRICT;

ALTER TABLE "SystemNotifications" ADD CONSTRAINT "FK_SystemNotifications_Users_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
```

---

## 🔧 Connection String Configuration

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hniDashOps;Username=postgres;Password=password;Port=5432",
    "Development": "Host=localhost;Database=hniDashOps_dev;Username=postgres;Password=password;Port=5432",
    "Staging": "Host=staging-db.railway.app;Database=hniDashOps_staging;Username=postgres;Password=staging_password;Port=5432",
    "Production": "Host=prod-db.railway.app;Database=hniDashOps_prod;Username=postgres;Password=prod_password;Port=5432"
  }
}
```

### **Program.cs Configuration**

```csharp
// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (builder.Environment.IsDevelopment())
    {
        connectionString = builder.Configuration.GetConnectionString("Development");
    }
    else if (builder.Environment.IsStaging())
    {
        connectionString = builder.Configuration.GetConnectionString("Staging");
    }
    else if (builder.Environment.IsProduction())
    {
        connectionString = builder.Configuration.GetConnectionString("Production");
    }
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
        npgsqlOptions.CommandTimeout(30);
    });
});
```

---

## 🚀 Deployment Migration

### **Railway Deployment**

```yaml
# railway.toml
[build]
builder = "NIXPACKS"

[deploy]
startCommand = "dotnet HniDashOps.API.dll"
healthcheckPath = "/health"
healthcheckTimeout = 300
restartPolicyType = "ON_FAILURE"
restartPolicyMaxRetries = 10

[env]
ASPNETCORE_ENVIRONMENT = "Production"
DATABASE_URL = "${{Postgres.DATABASE_URL}}"
```

### **Migration Script for Production**

```bash
#!/bin/bash
# scripts/migrate-production.sh

echo "🚀 Starting production migration..."

# Check if DATABASE_URL is set
if [ -z "$DATABASE_URL" ]; then
    echo "❌ DATABASE_URL environment variable is not set"
    exit 1
fi

# Run migrations
echo "📊 Running database migrations..."
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API --verbose

# Verify migration
echo "✅ Verifying migration..."
dotnet ef migrations list --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

echo "🎉 Migration completed successfully!"
```

---

## 🛡️ Migration Best Practices

### **✅ Do's**

```yaml
✅ Always backup database before migration
✅ Test migrations on staging first
✅ Use descriptive migration names
✅ Keep migrations small and focused
✅ Use data migrations for complex changes
✅ Monitor migration performance
✅ Have rollback plan ready
✅ Document breaking changes
```

### **❌ Don'ts**

```yaml
❌ Don't modify existing migrations
❌ Don't run migrations without testing
❌ Don't ignore migration errors
❌ Don't run migrations during peak hours
❌ Don't skip backup before migration
❌ Don't mix database changes with code changes
❌ Don't ignore foreign key constraints
❌ Don't forget to update connection strings
```

### **🔍 Migration Validation**

```csharp
// Add validation after migration
public class MigrationValidator
{
    private readonly ApplicationDbContext _context;
    
    public MigrationValidator(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> ValidateMigrationAsync()
    {
        try
        {
            // Test database connection
            await _context.Database.CanConnectAsync();
            
            // Test basic queries
            var userCount = await _context.Users.CountAsync();
            var menuCount = await _context.Menus.CountAsync();
            
            // Validate relationships
            var usersWithMenus = await _context.Users
                .Include(u => u.Menus)
                .Where(u => u.Menus.Any())
                .CountAsync();
                
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration validation failed: {ex.Message}");
            return false;
        }
    }
}
```

---

## 🚨 Troubleshooting

### **Common Issues**

#### **1. Connection String Issues**
```bash
# Error: Connection string not found
# Solution: Check appsettings.json and environment variables
dotnet user-secrets list
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

#### **2. Migration Conflicts**
```bash
# Error: Migration already exists
# Solution: Remove conflicting migration
dotnet ef migrations remove --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

#### **3. Database Schema Mismatch**
```bash
# Error: Schema mismatch
# Solution: Reset database and reapply migrations
dotnet ef database drop --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

#### **4. Permission Issues**
```bash
# Error: Permission denied
# Solution: Check database user permissions
GRANT ALL PRIVILEGES ON DATABASE hniDashOps TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
```

---

## 📚 Additional Resources

### **Documentation Links**
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Railway Documentation](https://docs.railway.app/)

### **Useful Commands**
```bash
# Check EF Core version
dotnet ef --version

# List all EF Core tools
dotnet tool list -g

# Update EF Core tools
dotnet tool update --global dotnet-ef

# Check database connection
dotnet ef database update --dry-run
```

---

## 🎯 Summary

### **Migration Strategy cho HniDashOps:**

1. **Existing Database**: Sử dụng Database-First approach
2. **New Database**: Sử dụng Code-First approach  
3. **Hybrid**: Kết hợp cả hai cho flexibility
4. **Deployment**: Automated migration với Railway
5. **Monitoring**: Validation và rollback plans

### **Next Steps:**

1. **Choose migration strategy** based on current database state
2. **Setup Entity Framework** với appropriate approach
3. **Create/Scaffold models** từ database
4. **Test migrations** trên development environment
5. **Deploy to staging** và production

---

**Happy Migrating! 🚀**

*Tài liệu này sẽ được cập nhật khi có thay đổi về database schema hoặc migration strategy.*
