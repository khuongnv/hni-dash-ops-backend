# 🚀 HniDashOps Backend Setup Guide

## 📋 Chuẩn Bị Cursor cho .NET Core Development

### **🎯 Phương Án B: Mở Workspace Mới (Recommended)**

---

## **1. 🎯 Tạo Workspace Mới**

### **Step 1: Tạo folder mới**
```bash
# Tạo folder backend riêng biệt
mkdir HniDashOps-Backend
cd HniDashOps-Backend

# Mở trong Cursor
cursor .
```

### **Step 2: Cấu trúc folder chuẩn bị**
```
HniDashOps-Backend/
├── docs/
│   ├── SETUP_GUIDE.md          # Hướng dẫn setup
│   ├── API_DOCUMENTATION.md    # Tài liệu API
│   └── DEPLOYMENT_GUIDE.md     # Hướng dẫn deploy
├── src/
│   ├── HniDashOps.API/         # Web API Project
│   ├── HniDashOps.Core/        # Business Logic
│   ├── HniDashOps.Infrastructure/ # Data Access
│   └── HniDashOps.Shared/      # Shared Models
├── tests/
│   ├── HniDashOps.API.Tests/
│   └── HniDashOps.Core.Tests/
├── scripts/
│   ├── setup.sh               # Setup script
│   └── deploy.sh              # Deploy script
├── .vscode/
│   ├── settings.json          # Cursor workspace settings
│   └── launch.json            # Debug configuration
└── README.md
```

---

## **2. 🔧 Cài Đặt Extensions Cursor**

### **Required Extensions:**
```yaml
✅ C# Dev Kit (Microsoft)
✅ .NET Install Tool
✅ NuGet Package Manager
✅ GitLens
✅ REST Client
✅ Thunder Client (API testing)
✅ PostgreSQL (Database support)
```

### **Install Commands:**
```bash
# Cài đặt extensions
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.vscode-dotnet-runtime
code --install-extension jmrog.vscode-nuget-package-manager
code --install-extension eamodio.gitlens
code --install-extension humao.rest-client
code --install-extension rangav.vscode-thunder-client
code --install-extension ms-ossdata.vscode-postgresql
```

---

## **3. 🚀 Setup Scripts**

### **Tạo file `scripts/setup.sh`:**
```bash
#!/bin/bash

echo "🚀 Setting up HniDashOps Backend..."

# Check .NET 9 installation
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 9 not found. Please install .NET 9 SDK"
    echo "📥 Download from: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "✅ .NET version: $DOTNET_VERSION"

# Create solution
echo "📁 Creating solution..."
dotnet new sln -n HniDashOps

# Create projects
echo "🏗️ Creating projects..."
dotnet new webapi -n HniDashOps.API -o src/HniDashOps.API
dotnet new classlib -n HniDashOps.Core -o src/HniDashOps.Core
dotnet new classlib -n HniDashOps.Infrastructure -o src/HniDashOps.Infrastructure
dotnet new classlib -n HniDashOps.Shared -o src/HniDashOps.Shared

# Add projects to solution
echo "🔗 Adding projects to solution..."
dotnet sln add src/HniDashOps.API/HniDashOps.API.csproj
dotnet sln add src/HniDashOps.Core/HniDashOps.Core.csproj
dotnet sln add src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj
dotnet sln add src/HniDashOps.Shared/HniDashOps.Shared.csproj

# Add project references
echo "🔗 Adding project references..."
dotnet add src/HniDashOps.API/HniDashOps.API.csproj reference src/HniDashOps.Core/HniDashOps.Core.csproj
dotnet add src/HniDashOps.API/HniDashOps.API.csproj reference src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj
dotnet add src/HniDashOps.API/HniDashOps.API.csproj reference src/HniDashOps.Shared/HniDashOps.Shared.csproj
dotnet add src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj reference src/HniDashOps.Core/HniDashOps.Core.csproj
dotnet add src/HniDashOps.Infrastructure/HniDashOps.Infrastructure.csproj reference src/HniDashOps.Shared/HniDashOps.Shared.csproj
dotnet add src/HniDashOps.Core/HniDashOps.Core.csproj reference src/HniDashOps.Shared/HniDashOps.Shared.csproj

# Create test projects
echo "🧪 Creating test projects..."
dotnet new xunit -n HniDashOps.API.Tests -o tests/HniDashOps.API.Tests
dotnet new xunit -n HniDashOps.Core.Tests -o tests/HniDashOps.Core.Tests

# Add test projects to solution
dotnet sln add tests/HniDashOps.API.Tests/HniDashOps.API.Tests.csproj
dotnet sln add tests/HniDashOps.Core.Tests/HniDashOps.Core.Tests.csproj

# Add test references
dotnet add tests/HniDashOps.API.Tests/HniDashOps.API.Tests.csproj reference src/HniDashOps.API/HniDashOps.API.csproj
dotnet add tests/HniDashOps.Core.Tests/HniDashOps.Core.Tests.csproj reference src/HniDashOps.Core/HniDashOps.Core.csproj

echo "✅ Setup completed!"
echo "📁 Project structure created"
echo "🚀 Ready to start development!"
echo ""
echo "Next steps:"
echo "1. Run: dotnet restore"
echo "2. Run: dotnet build"
echo "3. Run: dotnet run --project src/HniDashOps.API"
```

---

## **4. 📚 Tạo Documentation Files**

### **Tạo file `docs/SETUP_GUIDE.md`:**
```markdown
# HniDashOps Backend Setup Guide

## Prerequisites
- .NET 9 SDK
- PostgreSQL (local or Railway)
- Git
- Cursor IDE

## Quick Start
1. Run setup script: `./scripts/setup.sh`
2. Install NuGet packages: `dotnet restore`
3. Configure database connection
4. Run: `dotnet run --project src/HniDashOps.API`

## Project Structure
- `HniDashOps.API`: Web API controllers
- `HniDashOps.Core`: Business logic
- `HniDashOps.Infrastructure`: Data access
- `HniDashOps.Shared`: Shared models

## Next Steps
1. Configure Entity Framework
2. Setup Swagger
3. Implement Authentication
4. Create API endpoints
```

### **Tạo file `README.md`:**
```markdown
# HniDashOps Backend

.NET Core 9 Web API với Clean Architecture

## Features
- ✅ Authentication & Authorization
- ✅ User Management
- ✅ Menu Management
- ✅ Categories & Departments
- ✅ System Notifications
- ✅ Swagger Documentation
- ✅ PostgreSQL Database
- ✅ Railway Deployment

## Quick Start
```bash
# Setup project
./scripts/setup.sh

# Install packages
dotnet restore

# Run API
dotnet run --project src/HniDashOps.API
```

## API Documentation
- Swagger UI: `https://localhost:7000/swagger`
- API Base URL: `https://localhost:7000/api`

## Deployment
- Platform: Railway
- Database: PostgreSQL
- Frontend: Vercel (Nuxt.js)
```

---

## **5. 🎯 Cursor Workspace Configuration**

### **Tạo file `.vscode/settings.json`:**
```json
{
  "dotnet.defaultSolution": "HniDashOps.sln",
  "dotnet.completion.showCompletionItemsFromUnimportedNamespaces": true,
  "dotnet.inlayHints.enableInlayHintsForParameters": true,
  "dotnet.inlayHints.enableInlayHintsForLiteralParameters": true,
  "dotnet.inlayHints.enableInlayHintsForIndexerParameters": true,
  "dotnet.inlayHints.enableInlayHintsForObjectCreationParameters": true,
  "dotnet.inlayHints.enableInlayHintsForOtherParameters": true,
  "dotnet.inlayHints.suppressInlayHintsForParametersThatDifferOnlyBySuffix": true,
  "dotnet.inlayHints.suppressInlayHintsForParametersThatMatchMethodIntent": true,
  "dotnet.inlayHints.suppressInlayHintsForParametersThatMatchArgumentName": true,
  "files.exclude": {
    "**/bin": true,
    "**/obj": true
  },
  "search.exclude": {
    "**/bin": true,
    "**/obj": true
  }
}
```

### **Tạo file `.vscode/launch.json`:**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch API",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/HniDashOps.API/bin/Debug/net9.0/HniDashOps.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/HniDashOps.API",
      "console": "internalConsole",
      "stopAtEntry": false,
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

### **Tạo file `.vscode/tasks.json`:**
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/src/HniDashOps.API/HniDashOps.API.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "publish",
      "command": "dotnet",
      "type": "process",
      "args": [
        "publish",
        "${workspaceFolder}/src/HniDashOps.API/HniDashOps.API.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "watch",
      "command": "dotnet",
      "type": "process",
      "args": [
        "watch",
        "run",
        "--project",
        "${workspaceFolder}/src/HniDashOps.API/HniDashOps.API.csproj"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

---

## **6. 🚀 Bắt Đầu Development**

### **Step 1: Tạo workspace mới**
```bash
# Tạo folder backend riêng biệt
mkdir HniDashOps-Backend
cd HniDashOps-Backend

# Mở trong Cursor
cursor .
```

### **Step 2: Tạo cấu trúc folder**
```bash
# Tạo các folder cần thiết
mkdir -p docs src tests scripts .vscode
```

### **Step 3: Tạo setup script**
```bash
# Tạo file setup.sh
touch scripts/setup.sh
chmod +x scripts/setup.sh

# Copy nội dung setup script vào file
```

### **Step 4: Chạy setup script**
```bash
# Chạy setup script
./scripts/setup.sh
```

### **Step 5: Mở file MD để rà soát**
```bash
# Mở các file documentation
cursor docs/SETUP_GUIDE.md
cursor BACKEND_MIGRATION_PLAN.md
cursor BACKEND_SETUP.md
```

### **Step 6: Bắt đầu development**
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
dotnet run --project src/HniDashOps.API
```

---

## **7. 📋 Checklist Hoàn Thành**

```yaml
✅ Tạo workspace mới (HniDashOps-Backend)
✅ Cài đặt C# extensions
✅ Tạo cấu trúc folder
✅ Tạo setup script
✅ Tạo documentation files
✅ Cấu hình Cursor workspace
✅ Chạy setup script
✅ Mở file MD để rà soát
✅ Bắt đầu development
```

---

## **8. 🎯 Next Steps**

### **Sau khi setup xong:**
1. **Configure Entity Framework** với PostgreSQL
2. **Setup Swagger** documentation
3. **Implement Authentication** APIs
4. **Create User Management** APIs
5. **Setup Railway** deployment
6. **Test integration** với Nuxt frontend

### **Development Timeline:**
```yaml
Week 1: Setup + Authentication APIs
Week 2: User Management + Menu APIs
Week 3: Categories + Departments APIs
Week 4: Testing + Deployment
```

---

## **9. 🔧 Troubleshooting**

### **Common Issues:**
```yaml
❌ .NET 9 not found
✅ Solution: Install .NET 9 SDK from Microsoft

❌ Permission denied on setup.sh
✅ Solution: chmod +x scripts/setup.sh

❌ NuGet packages not restored
✅ Solution: dotnet restore

❌ Build errors
✅ Solution: Check project references
```

---

*Tài liệu này hướng dẫn setup môi trường Cursor cho .NET Core 9 development với Clean Architecture.*
