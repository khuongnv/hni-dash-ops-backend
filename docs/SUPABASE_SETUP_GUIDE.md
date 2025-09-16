# 🔗 Supabase PostgreSQL Setup Guide

## 📋 Tổng Quan

Hướng dẫn chi tiết về cách setup và kết nối với Supabase PostgreSQL database cho dự án HniDashOps Backend.

## 🎯 Supabase là gì?

Supabase là một open-source Firebase alternative với:
- **PostgreSQL Database** (managed)
- **Real-time subscriptions**
- **Authentication**
- **API auto-generation**
- **Dashboard interface**
- **Free tier** với generous limits

## 🚀 Tạo Supabase Project

### **Step 1: Đăng ký Supabase**
1. Truy cập [Supabase](https://supabase.com)
2. Click **Start your project**
3. Đăng nhập với GitHub/Google
4. Click **New Project**

### **Step 2: Tạo Project**
```yaml
Project Name: hni-dash-ops
Database Password: [Generate strong password]
Region: Southeast Asia (Singapore)
Pricing Plan: Free
```

### **Step 3: Chờ Setup**
- Supabase sẽ tạo project trong 2-3 phút
- Database sẽ được setup tự động
- Bạn sẽ nhận được email confirmation

## 🔗 Lấy Connection String

### **Method 1: Từ Dashboard**

#### **Step 1: Truy cập Database Settings**
1. Vào Supabase Dashboard
2. Chọn project của bạn
3. Vào **Settings** → **Database**

#### **Step 2: Lấy Connection Info**
```yaml
Connection Info:
- Host: db.xxxxxxxxxxxxxxxx.supabase.co
- Database name: postgres
- Port: 5432
- User: postgres
- Password: [Your database password]
```

#### **Step 3: Tạo Connection String**
```bash
# Format cho .NET
Host=db.xxxxxxxxxxxxxxxx.supabase.co;Database=postgres;Username=postgres;Password=your_password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

### **Method 2: Từ Project Settings**

#### **Step 1: Vào API Settings**
1. **Settings** → **API**
2. Scroll xuống phần **Database**
3. Copy **Connection string**

#### **Step 2: Format Connection String**
```bash
# PostgreSQL URL format
postgresql://postgres:password@db.xxx.supabase.co:5432/postgres

# Convert to .NET format
Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

## 🔧 Cấu Hình cho HniDashOps

### **Step 1: Cập nhật appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxxxxxxxxxxxxxxx.supabase.co;Database=postgres;Username=postgres;Password=your_password;Port=5432;SSL Mode=Require;Trust Server Certificate=true",
    "Supabase": "Host=db.xxxxxxxxxxxxxxxx.supabase.co;Database=postgres;Username=postgres;Password=your_password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Supabase": {
    "Url": "https://xxxxxxxxxxxxxxxx.supabase.co",
    "AnonKey": "your-anon-key",
    "ServiceRoleKey": "your-service-role-key"
  }
}
```

### **Step 2: Environment Variables**

```bash
# Development
export DATABASE_URL="Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432"
export SUPABASE_URL="https://xxx.supabase.co"
export SUPABASE_ANON_KEY="your-anon-key"

# Production (Railway)
DATABASE_URL=Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432
SUPABASE_URL=https://xxx.supabase.co
SUPABASE_ANON_KEY=your-anon-key
```

### **Step 3: Program.cs Configuration**

```csharp
// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30));
        npgsqlOptions.CommandTimeout(30);
    });
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

## 🗄️ Database Migration với Supabase

### **Step 1: Scaffold từ Existing Database**

```bash
# Nếu bạn đã có tables trong Supabase
dotnet ef dbcontext scaffold "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c ApplicationDbContext --force
```

### **Step 2: Code-First Migration**

```bash
# Tạo migration mới
dotnet ef migrations add InitialCreate --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Apply migration to Supabase
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

### **Step 3: Verify Migration**

```bash
# Check migration status
dotnet ef migrations list --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Test connection
dotnet ef database update --dry-run --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

## 🧪 Test Connection

### **Method 1: Sử dụng Test Script**

```bash
# Set environment variable
export DATABASE_URL="Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432"

# Run test script
./scripts/test-supabase-connection.sh
```

### **Method 2: Manual Test**

```bash
# Test với psql
psql "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require" -c "SELECT version();"

# Test với .NET
dotnet run --project src/HniDashOps.API
# Visit: https://localhost:7000/health
```

### **Method 3: Supabase Dashboard**

1. Vào Supabase Dashboard
2. **Table Editor** → **New table**
3. Tạo test table để verify connection

## 🔐 Security Best Practices

### **1. Environment Variables**
```yaml
✅ Use environment variables for sensitive data
✅ Never commit passwords to Git
✅ Use different credentials for dev/staging/prod
✅ Rotate passwords regularly
```

### **2. Connection Security**
```yaml
✅ Always use SSL (SSL Mode=Require)
✅ Trust Server Certificate=true for Supabase
✅ Use connection pooling
✅ Enable retry on failure
```

### **3. Database Security**
```yaml
✅ Use Row Level Security (RLS) in Supabase
✅ Create specific database users for different purposes
✅ Enable audit logging
✅ Regular security updates
```

## 🚀 Deployment với Railway

### **Step 1: Railway Configuration**

```yaml
# railway.toml
[build]
builder = "NIXPACKS"

[deploy]
startCommand = "dotnet HniDashOps.API.dll"
healthcheckPath = "/health"

[env]
ASPNETCORE_ENVIRONMENT = "Production"
DATABASE_URL = "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
SUPABASE_URL = "https://xxx.supabase.co"
SUPABASE_ANON_KEY = "your-anon-key"
```

### **Step 2: Environment Variables trong Railway**

```bash
# Set in Railway dashboard
DATABASE_URL=Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
SUPABASE_URL=https://xxx.supabase.co
SUPABASE_ANON_KEY=your-anon-key
JWT_SECRET=your-production-jwt-secret
```

### **Step 3: Deploy**

```bash
# Push to GitHub (Railway auto-deploys)
git add .
git commit -m "Add Supabase configuration"
git push origin main
```

## 📊 Supabase Features cho HniDashOps

### **1. Database Features**
```yaml
✅ PostgreSQL 15+ with extensions
✅ Real-time subscriptions
✅ Full-text search
✅ JSON/JSONB support
✅ Row Level Security (RLS)
✅ Database backups
```

### **2. API Features**
```yaml
✅ Auto-generated REST API
✅ GraphQL support
✅ Real-time subscriptions
✅ Authentication integration
✅ File storage
✅ Edge functions
```

### **3. Dashboard Features**
```yaml
✅ Table editor
✅ SQL editor
✅ Database logs
✅ Performance monitoring
✅ User management
✅ API documentation
```

## 🔍 Troubleshooting

### **Common Issues**

#### **1. Connection Timeout**
```bash
# Error: Connection timeout
# Solution: Check network connectivity and firewall
ping db.xxx.supabase.co
telnet db.xxx.supabase.co 5432
```

#### **2. SSL Certificate Issues**
```bash
# Error: SSL certificate verification failed
# Solution: Add Trust Server Certificate=true
Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

#### **3. Authentication Failed**
```bash
# Error: Authentication failed
# Solution: Check username/password
# Reset password in Supabase Dashboard → Settings → Database
```

#### **4. Database Not Found**
```bash
# Error: Database does not exist
# Solution: Use 'postgres' as database name (default for Supabase)
Database=postgres
```

### **Debug Commands**

```bash
# Check connection string format
echo $DATABASE_URL

# Test with psql
psql "$DATABASE_URL" -c "SELECT 1;"

# Test with .NET
dotnet ef database update --dry-run

# Check Supabase status
curl -I https://xxx.supabase.co/rest/v1/
```

## 📚 Additional Resources

### **Documentation**
- [Supabase Documentation](https://supabase.com/docs)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### **Useful Links**
- [Supabase Dashboard](https://app.supabase.com)
- [Supabase Community](https://github.com/supabase/supabase)
- [PostgreSQL Extensions](https://supabase.com/docs/guides/database/extensions)

## 🎯 Next Steps

1. **Setup Supabase project** và lấy connection string
2. **Configure appsettings.json** với Supabase credentials
3. **Test connection** với provided scripts
4. **Run database migration** (Database-First hoặc Code-First)
5. **Deploy to Railway** với Supabase integration

---

**Happy Coding with Supabase! 🚀**

*Tài liệu này sẽ được cập nhật khi có thay đổi về Supabase features hoặc configuration.*
