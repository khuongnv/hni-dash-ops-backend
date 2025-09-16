# HniDashOps Backend Setup Guide

## 📋 Prerequisites

### Required Software:
- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL** (local or Railway managed)
- **Redis** (for session management)
- **Cursor IDE** với C# extensions
- **Git** for version control

### Cursor Extensions:
```yaml
✅ C# Dev Kit (Microsoft)
✅ .NET Install Tool
✅ NuGet Package Manager
✅ GitLens
✅ REST Client
✅ Thunder Client (API testing)
✅ PostgreSQL (Database support)
```

## 🚀 Installation Steps

### Step 1: Install .NET 9 SDK

#### macOS:
```bash
# Using Homebrew
brew install dotnet

# Or download from Microsoft
# https://dotnet.microsoft.com/download/dotnet/9.0
```

#### Windows:
```bash
# Using winget
winget install Microsoft.DotNet.SDK.9

# Or download installer from Microsoft
```

#### Linux (Ubuntu/Debian):
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Install .NET 9 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```

### Step 2: Verify Installation
```bash
# Check .NET version
dotnet --version
# Should output: 9.0.x

# Check SDK info
dotnet --info
```

### Step 3: Setup Project

```bash
# Clone repository (if not already done)
git clone <repository-url>
cd HniDashOps-Backend

# Make setup script executable
chmod +x scripts/setup.sh

# Run setup script
./scripts/setup.sh

# Install NuGet packages
dotnet restore

# Build solution
dotnet build
```

### Step 4: Configure Database

#### Option A: Local PostgreSQL
```bash
# Install PostgreSQL
brew install postgresql  # macOS
sudo apt install postgresql  # Ubuntu

# Start PostgreSQL service
brew services start postgresql  # macOS
sudo systemctl start postgresql  # Ubuntu

# Create database
createdb hniDashOps
```

#### Option B: Railway PostgreSQL (Recommended)
1. Go to [Railway.app](https://railway.app)
2. Create new project
3. Add PostgreSQL service
4. Copy connection string to environment variables

### Step 5: Configure Environment Variables

Create `src/HniDashOps.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hniDashOps;Username=postgres;Password=password",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "your-super-secret-jwt-key-here",
    "ExpiryMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://localhost:3000"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Step 6: Run Database Migrations

```bash
# Add Entity Framework tools (if not already added)
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Apply migrations to database
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

### Step 7: Run Application

```bash
# Run API server
dotnet run --project src/HniDashOps.API

# Or use watch mode for development
dotnet watch run --project src/HniDashOps.API
```

### Step 8: Verify Setup

1. **API Health Check**: http://localhost:7000/health
2. **Swagger UI**: http://localhost:7000/swagger
3. **API Base URL**: http://localhost:7000/api

## 🔧 Development Workflow

### Daily Development:
```bash
# Start development server
dotnet watch run --project src/HniDashOps.API

# Run tests
dotnet test

# Build for production
dotnet build --configuration Release

# Publish for deployment
dotnet publish src/HniDashOps.API --configuration Release --output ./publish
```

### Database Operations:
```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Update database
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Remove last migration
dotnet ef migrations remove --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

## 🧪 Testing

### Unit Tests:
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/HniDashOps.API.Tests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### API Testing:
- **Swagger UI**: http://localhost:7000/swagger
- **Thunder Client**: Import API collection
- **REST Client**: Use .http files in tests folder

## 🚀 Deployment

### Railway Deployment:

1. **Connect Repository**:
   - Go to [Railway.app](https://railway.app)
   - Connect GitHub repository
   - Select HniDashOps-Backend repository

2. **Add Services**:
   - Add .NET service
   - Add PostgreSQL service
   - Add Redis service (optional)

3. **Configure Environment**:
   ```bash
   DATABASE_URL=postgresql://username:password@host:port/database
   REDIS_URL=redis://username:password@host:port
   JWT_SECRET=your-production-jwt-secret
   ASPNETCORE_ENVIRONMENT=Production
   ```

4. **Deploy**:
   - Push to main branch
   - Railway auto-deploys

## 🔍 Troubleshooting

### Common Issues:

#### .NET 9 Not Found:
```bash
# Check if .NET is installed
dotnet --version

# If not installed, install .NET 9 SDK
# macOS: brew install dotnet
# Windows: winget install Microsoft.DotNet.SDK.9
# Linux: Follow installation steps above
```

#### Database Connection Issues:
```bash
# Check PostgreSQL status
brew services list | grep postgresql  # macOS
sudo systemctl status postgresql     # Ubuntu

# Test connection
psql -h localhost -U postgres -d hniDashOps
```

#### Build Errors:
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# Check for missing packages
dotnet list package --outdated
```

#### Port Already in Use:
```bash
# Find process using port 7000
lsof -i :7000  # macOS/Linux
netstat -ano | findstr :7000  # Windows

# Kill process
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

### Getting Help:

1. **Check Logs**: Application logs in console output
2. **Swagger UI**: Test API endpoints
3. **Health Check**: http://localhost:7000/health
4. **GitHub Issues**: Report bugs or ask questions
5. **Documentation**: Check docs/ folder for more details

## 📚 Next Steps

After successful setup:

1. **Implement Authentication APIs**
2. **Create User Management endpoints**
3. **Setup Menu Management system**
4. **Add Categories & Departments**
5. **Implement System Notifications**
6. **Test with Nuxt frontend**
7. **Deploy to Railway**

## 📞 Support

- **Documentation**: Check docs/ folder
- **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- **Discussions**: [GitHub Discussions](https://github.com/your-repo/discussions)

---

**Happy Coding! 🚀**
