# HniDashOps Backend

.NET 9 Web API với Clean Architecture cho hệ thống quản lý HniDashOps.

## 🚀 Features

- ✅ **Authentication & Authorization** (JWT + Role-based)
- ✅ **User Management** (CRUD operations)
- ✅ **Role & Permission Management** (RBAC system)
- ✅ **Menu Management** (Hierarchical menu system)
- ✅ **Categories Management** (Hierarchical categories)
- ✅ **Departments Management** (Organizational structure)
- ✅ **System Notifications** (Real-time notifications)
- ✅ **Swagger Documentation** (Interactive API docs)
- ✅ **PostgreSQL Database** (Supabase managed)
- ✅ **Entity Framework Core** (Code-first migrations)
- ✅ **UPPER_CASE Database Naming** (Oracle compatibility)

## 📁 Project Structure

```
HniDashOps-Backend/
├── src/
│   ├── HniDashOps.API/           # Web API Controllers & DTOs
│   │   ├── Controllers/          # API Controllers
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   └── Program.cs            # Application entry point
│   ├── HniDashOps.Core/          # Business Logic & Entities
│   │   ├── Entities/             # Domain entities
│   │   └── Services/             # Service interfaces
│   ├── HniDashOps.Infrastructure/ # Data Access Layer
│   │   ├── Data/                 # DbContext & Seed data
│   │   ├── Services/             # Service implementations
│   │   └── Migrations/           # EF Core migrations
│   └── HniDashOps.Shared/        # Shared models
├── scripts/                      # Setup scripts
├── docs/                         # Documentation
└── HniDashOps.sln               # Solution file
```

## 🛠️ Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL** (Supabase recommended)
- **Visual Studio 2022** hoặc **VS Code** với C# extensions

## 🚀 Quick Start

### 1. Install .NET 9 SDK

```bash
# Windows
winget install Microsoft.DotNet.SDK.9

# macOS
brew install dotnet

# Linux
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```

### 2. Setup Project

```bash
# Clone repository
git clone <repository-url>
cd hni-dash-ops-backend

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
dotnet run --project src/HniDashOps.API
```

### 3. Database Setup

```bash
# Apply migrations
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API

# Seed initial data
# Access: https://localhost:64706/api/seed/seed
```

### 4. Access API

- **API Base URL**: `https://localhost:64706/api`
- **Swagger UI**: `https://localhost:64706/swagger`
- **Health Check**: `https://localhost:64706/health`

## 📚 API Documentation

### Authentication APIs
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/change-password` - Change password

### User Management APIs
- `GET /api/users` - Get users list
- `POST /api/users` - Create new user
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `POST /api/users/{id}/assign-role` - Assign role to user

### Role Management APIs
- `GET /api/roles` - Get roles list
- `POST /api/roles` - Create new role
- `GET /api/roles/{id}` - Get role by ID
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role
- `POST /api/roles/{id}/assign-permissions` - Assign permissions to role

### Permission Management APIs
- `GET /api/permissions` - Get permissions list
- `POST /api/permissions` - Create new permission
- `GET /api/permissions/{id}` - Get permission by ID
- `PUT /api/permissions/{id}` - Update permission
- `DELETE /api/permissions/{id}` - Delete permission

### Menu Management APIs
- `GET /api/menus` - Get menus list
- `POST /api/menus` - Create new menu
- `GET /api/menus/{id}` - Get menu by ID
- `PUT /api/menus/{id}` - Update menu
- `DELETE /api/menus/{id}` - Delete menu
- `GET /api/menus/hierarchy` - Get hierarchical menus
- `POST /api/menus/reorder` - Reorder menus

### Categories APIs
- `GET /api/categories` - Get categories list
- `POST /api/categories` - Create new category
- `GET /api/categories/{id}` - Get category by ID
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category
- `GET /api/categories/hierarchy` - Get hierarchical categories
- `POST /api/categories/reorder` - Reorder categories

### Departments APIs
- `GET /api/departments` - Get departments list
- `POST /api/departments` - Create new department
- `GET /api/departments/{id}` - Get department by ID
- `PUT /api/departments/{id}` - Update department
- `DELETE /api/departments/{id}` - Delete department
- `GET /api/departments/hierarchy` - Get hierarchical departments
- `POST /api/departments/{id}/assign-user` - Assign user to department

### System Notifications APIs
- `GET /api/systemnotifications` - Get notifications list
- `POST /api/systemnotifications` - Create new notification
- `GET /api/systemnotifications/{id}` - Get notification by ID
- `PUT /api/systemnotifications/{id}` - Update notification
- `DELETE /api/systemnotifications/{id}` - Delete notification
- `POST /api/systemnotifications/{id}/mark-read` - Mark as read
- `GET /api/systemnotifications/active` - Get active notifications

### Seed Data APIs
- `POST /api/seed/seed` - Seed initial data (roles, permissions, users)

## 🗄️ Database Schema

### Core Tables (UPPER_CASE naming)

- **USERS** - User accounts
- **ROLES** - User roles
- **PERMISSIONS** - System permissions
- **USERROLES** - User-role assignments
- **ROLEPERMISSIONS** - Role-permission assignments

### Business Tables

- **DEPARTMENTS** - Organizational departments
- **MENUS** - System menu structure
- **CATEGORIES** - Content categories
- **SYSTEM_NOTIFICATIONS** - System notifications

### Base Entity Fields

All entities inherit from `BaseEntity`:
- `ID` - Primary key
- `ISACTIVE` - Active status
- `CREATEDAT` - Creation timestamp
- `UPDATEDAT` - Last update timestamp
- `ISDELETED` - Soft delete flag

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-postgresql-connection-string"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "HniDashOps",
    "Audience": "HniDashOps-Users",
    "ExpiryMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": ["https://localhost:3000"]
  }
}
```

## 🚀 Deployment

### Local Development

```bash
# Run with specific port
dotnet run --project src/HniDashOps.API --urls="https://localhost:64706"
```

### Production Deployment

1. **Configure connection string**
2. **Set JWT secret key**
3. **Configure CORS origins**
4. **Apply database migrations**
5. **Seed initial data**

## 🧪 Testing

```bash
# Build solution
dotnet build

# Run specific project
dotnet run --project src/HniDashOps.API

# Check database connection
# Access: https://localhost:64706/api/seed/seed
```

## 🔒 Security Features

- **JWT Authentication** với role-based authorization
- **Input Validation** với Data Annotations
- **CORS** configuration
- **HTTPS** enforced
- **Password Hashing** với SHA256
- **Soft Delete** pattern

## 📊 Performance

- **Entity Framework Core** với connection pooling
- **Async/Await** pattern throughout
- **Pagination** support
- **Hierarchical queries** optimized
- **Indexed database** columns

## 🗂️ DTOs Structure

DTOs are organized by domain:
- `UserDTOs.cs` - User-related DTOs
- `RoleDTOs.cs` - Role-related DTOs
- `PermissionDTOs.cs` - Permission-related DTOs
- `DepartmentDTOs.cs` - Department-related DTOs
- `MenuDTOs.cs` - Menu-related DTOs
- `CategoryDTOs.cs` - Category-related DTOs
- `SystemNotificationDTOs.cs` - Notification-related DTOs

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

- **Documentation**: [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/your-repo/issues)

---

**Built with ❤️ using .NET 9, Entity Framework Core, PostgreSQL, and Clean Architecture**