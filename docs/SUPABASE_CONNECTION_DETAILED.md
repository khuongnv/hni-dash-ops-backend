# 🔗 Hướng Dẫn Chi Tiết Lấy Connection String từ Supabase

## 📋 Tổng Quan

Hướng dẫn step-by-step chi tiết với screenshots và examples cụ thể để lấy connection string từ Supabase PostgreSQL database.

## 🎯 Prerequisites

Trước khi bắt đầu, bạn cần:
- ✅ Supabase account (free)
- ✅ Supabase project đã được tạo
- ✅ Database password (đã set khi tạo project)

## 🚀 Method 1: Từ Database Settings (Recommended)

### **Step 1: Đăng nhập Supabase Dashboard**

1. Truy cập [https://app.supabase.com](https://app.supabase.com)
2. Đăng nhập với GitHub/Google account
3. Chọn project của bạn từ danh sách

### **Step 2: Vào Database Settings**

1. Trong project dashboard, click vào **Settings** (biểu tượng ⚙️) ở sidebar bên trái
2. Click vào **Database** trong menu Settings

### **Step 3: Lấy Connection Information**

Trong Database settings, bạn sẽ thấy section **Connection Info**:

```yaml
Connection Info:
┌─────────────────────────────────────────────────────────┐
│ Host: db.xxxxxxxxxxxxxxxx.supabase.co                   │
│ Database name: postgres                                 │
│ Port: 5432                                              │
│ User: postgres                                          │
│ Password: [Your database password]                      │
│ Connection string: postgresql://postgres:***@db...      │
└─────────────────────────────────────────────────────────┘
```

### **Step 4: Copy Connection String**

Click vào **Copy** button bên cạnh connection string để copy:

```bash
# Raw PostgreSQL connection string
postgresql://postgres:[YOUR-PASSWORD]@db.xxxxxxxxxxxxxxxx.supabase.co:5432/postgres
```

### **Step 5: Convert sang .NET Format**

Convert PostgreSQL URL sang .NET connection string format:

```bash
# Input (PostgreSQL URL):
postgresql://postgres:password123@db.abcdefghijklmnop.supabase.co:5432/postgres

# Output (.NET Connection String):
Host=db.abcdefghijklmnop.supabase.co;Database=postgres;Username=postgres;Password=password123;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

---

## 🔧 Method 2: Từ API Settings

### **Step 1: Vào API Settings**

1. Trong project dashboard, click **Settings** → **API**
2. Scroll xuống phần **Database**

### **Step 2: Lấy Database Connection**

Trong API settings, bạn sẽ thấy:

```yaml
Database:
┌─────────────────────────────────────────────────────────┐
│ Connection string: postgresql://postgres:***@db...      │
│ Direct connection: db.xxx.supabase.co:5432             │
│ Session mode: Session                                    │
│ Pooler mode: Transaction                                 │
└─────────────────────────────────────────────────────────┘
```

### **Step 3: Copy và Format**

```bash
# Copy connection string và format cho .NET
Host=db.xxxxxxxxxxxxxxxx.supabase.co;Database=postgres;Username=postgres;Password=your_password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

---

## 🛠️ Method 3: Manual Assembly

### **Step 1: Lấy từng thông tin riêng lẻ**

Từ Database Settings, lấy từng thông tin:

```yaml
Host: db.xxxxxxxxxxxxxxxx.supabase.co
Database: postgres
Port: 5432
Username: postgres
Password: [Your database password]
```

### **Step 2: Assemble Connection String**

```bash
# Template
Host={HOST};Database={DATABASE};Username={USERNAME};Password={PASSWORD};Port={PORT};SSL Mode=Require;Trust Server Certificate=true

# Example
Host=db.abcdefghijklmnop.supabase.co;Database=postgres;Username=postgres;Password=MySecurePassword123;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

---

## 🔐 Method 4: Environment Variables

### **Step 1: Lấy từ Supabase Dashboard**

```yaml
# Từ Database Settings
SUPABASE_HOST=db.xxxxxxxxxxxxxxxx.supabase.co
SUPABASE_DB=postgres
SUPABASE_PORT=5432
SUPABASE_USER=postgres
SUPABASE_PASSWORD=your_password
```

### **Step 2: Tạo Connection String từ Environment Variables**

```bash
# In your application
Host=${SUPABASE_HOST};Database=${SUPABASE_DB};Username=${SUPABASE_USER};Password=${SUPABASE_PASSWORD};Port=${SUPABASE_PORT};SSL Mode=Require;Trust Server Certificate=true
```

---

## 📝 Connection String Examples

### **Development Environment**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.abcdefghijklmnop.supabase.co;Database=postgres;Username=postgres;Password=dev_password_123;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### **Production Environment**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.abcdefghijklmnop.supabase.co;Database=postgres;Username=postgres;Password=prod_password_456;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### **With Connection Pooling**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.abcdefghijklmnop.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;MinPoolSize=5;MaxPoolSize=100"
  }
}
```

---

## 🧪 Test Connection String

### **Method 1: Sử dụng psql**

```bash
# Test với psql command
psql "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true" -c "SELECT version();"

# Nếu thành công, bạn sẽ thấy:
# PostgreSQL 15.4 on x86_64-pc-linux-gnu, compiled by gcc (GCC) 9.4.0, 64-bit
```

### **Method 2: Sử dụng .NET EF Core**

```bash
# Test với Entity Framework
dotnet ef database update --dry-run --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Nếu thành công, sẽ không có error
```

### **Method 3: Sử dụng Test Script**

```bash
# Set environment variable
export DATABASE_URL="Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"

# Run test script
./scripts/test-supabase-connection.sh
```

---

## 🔍 Troubleshooting Connection Issues

### **Issue 1: SSL Certificate Error**

```bash
# Error: SSL certificate verification failed
# Solution: Add Trust Server Certificate=true
Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

### **Issue 2: Connection Timeout**

```bash
# Error: Connection timeout
# Solution: Check network và firewall
ping db.xxx.supabase.co
telnet db.xxx.supabase.co 5432

# Nếu không ping được, check firewall settings
```

### **Issue 3: Authentication Failed**

```bash
# Error: Authentication failed
# Solution: 
# 1. Check username/password
# 2. Reset password trong Supabase Dashboard
# 3. Verify database user permissions
```

### **Issue 4: Database Not Found**

```bash
# Error: Database does not exist
# Solution: Use 'postgres' as database name (default for Supabase)
Database=postgres
```

### **Issue 5: Invalid Connection String Format**

```bash
# Error: Invalid connection string
# Solution: Check format và special characters
# ❌ Wrong: Host=db.xxx.supabase.co Database=postgres
# ✅ Correct: Host=db.xxx.supabase.co;Database=postgres;
```

---

## 🛡️ Security Best Practices

### **1. Password Security**

```yaml
✅ Use strong passwords (12+ characters)
✅ Include numbers, symbols, uppercase, lowercase
✅ Don't use common passwords
✅ Rotate passwords regularly
✅ Never share passwords in plain text
```

### **2. Connection String Security**

```yaml
✅ Store in environment variables
✅ Use appsettings.Production.json for production
✅ Never commit passwords to Git
✅ Use different passwords for dev/staging/prod
✅ Enable SSL (SSL Mode=Require)
```

### **3. Network Security**

```yaml
✅ Use SSL connections only
✅ Trust Server Certificate for Supabase
✅ Use connection pooling
✅ Enable retry on failure
✅ Monitor connection logs
```

---

## 📊 Connection String Components Explained

### **Required Components**

```yaml
Host: Database server address
Database: Database name (always 'postgres' for Supabase)
Username: Database user (always 'postgres' for Supabase)
Password: Database password (set when creating project)
Port: Database port (always 5432 for PostgreSQL)
```

### **Optional Components**

```yaml
SSL Mode: Security mode (Require for Supabase)
Trust Server Certificate: Trust Supabase SSL certificate
Pooling: Connection pooling (true/false)
MinPoolSize: Minimum connections in pool
MaxPoolSize: Maximum connections in pool
CommandTimeout: Command timeout in seconds
```

### **Supabase Specific Components**

```yaml
SSL Mode=Require: Bắt buộc SSL
Trust Server Certificate=true: Trust Supabase certificate
Pooling=true: Enable connection pooling
MinPoolSize=5: Minimum 5 connections
MaxPoolSize=100: Maximum 100 connections
```

---

## 🔄 Connection String Templates

### **Basic Template**

```bash
Host={HOST};Database=postgres;Username=postgres;Password={PASSWORD};Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

### **With Pooling Template**

```bash
Host={HOST};Database=postgres;Username=postgres;Password={PASSWORD};Port=5432;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;MinPoolSize=5;MaxPoolSize=100
```

### **With Timeout Template**

```bash
Host={HOST};Database=postgres;Username=postgres;Password={PASSWORD};Port=5432;SSL Mode=Require;Trust Server Certificate=true;CommandTimeout=30
```

### **Complete Template**

```bash
Host={HOST};Database=postgres;Username=postgres;Password={PASSWORD};Port=5432;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;MinPoolSize=5;MaxPoolSize=100;CommandTimeout=30;Timeout=15
```

---

## 🚀 Quick Setup Script

### **Create Connection String Script**

```bash
#!/bin/bash
# scripts/create-connection-string.sh

echo "🔗 Supabase Connection String Generator"
echo "======================================"

# Get input from user
read -p "Enter Supabase Host (db.xxx.supabase.co): " HOST
read -p "Enter Database Password: " -s PASSWORD
echo ""

# Validate input
if [ -z "$HOST" ] || [ -z "$PASSWORD" ]; then
    echo "❌ Error: Host and Password are required"
    exit 1
fi

# Generate connection string
CONNECTION_STRING="Host=${HOST};Database=postgres;Username=postgres;Password=${PASSWORD};Port=5432;SSL Mode=Require;Trust Server Certificate=true"

echo ""
echo "✅ Generated Connection String:"
echo "================================"
echo "$CONNECTION_STRING"
echo ""

# Save to file
echo "$CONNECTION_STRING" > connection-string.txt
echo "💾 Saved to connection-string.txt"

# Test connection
echo ""
echo "🧪 Testing connection..."
psql "$CONNECTION_STRING" -c "SELECT 'Connection successful!' as status;" 2>/dev/null

if [ $? -eq 0 ]; then
    echo "✅ Connection test successful!"
else
    echo "❌ Connection test failed!"
    echo "Please check your credentials and network connectivity"
fi
```

### **Usage**

```bash
# Make script executable
chmod +x scripts/create-connection-string.sh

# Run script
./scripts/create-connection-string.sh

# Follow prompts to generate connection string
```

---

## 📱 Mobile/Web App Integration

### **For Mobile Apps**

```yaml
# Store in secure storage
Key: "supabase_connection_string"
Value: "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
```

### **For Web Apps**

```yaml
# Store in environment variables
DATABASE_URL=Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

### **For Serverless Functions**

```yaml
# Store in function environment
ConnectionString: "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
```

---

## 🎯 Summary

### **Quick Steps to Get Connection String:**

1. **Login to Supabase Dashboard**
2. **Go to Settings → Database**
3. **Copy Connection Info**
4. **Format for .NET:**
   ```bash
   Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
   ```
5. **Test connection**
6. **Use in appsettings.json**

### **Important Notes:**

```yaml
✅ Always use SSL Mode=Require
✅ Always use Trust Server Certificate=true
✅ Database name is always 'postgres'
✅ Username is always 'postgres'
✅ Port is always 5432
✅ Store password securely
✅ Test connection before using
```

---

**Happy Connecting! 🚀**

*Tài liệu này cung cấp hướng dẫn chi tiết nhất để lấy connection string từ Supabase PostgreSQL database.*
