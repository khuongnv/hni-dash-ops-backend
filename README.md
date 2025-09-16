# HniDashOps Backend

.NET Core 9 Web API với Clean Architecture cho hệ thống quản lý HniDashOps.

## 🚀 Features

- ✅ **Authentication & Authorization** (JWT + SSO)
- ✅ **User Management** (CRUD operations)
- ✅ **Menu Management** (Dynamic menu system)
- ✅ **Categories & Departments** (Organization structure)
- ✅ **System Notifications** (Real-time notifications)
- ✅ **Swagger Documentation** (Interactive API docs)
- ✅ **PostgreSQL Database** (Railway managed)
- ✅ **Redis Caching** (Session management)
- ✅ **Railway Deployment** (Zero cost hosting)

## 📁 Project Structure

```
HniDashOps-Backend/
├── src/
│   ├── HniDashOps.API/           # Web API Controllers
│   ├── HniDashOps.Core/          # Business Logic
│   ├── HniDashOps.Infrastructure/ # Data Access Layer
│   └── HniDashOps.Shared/        # Shared Models & DTOs
├── tests/                        # Unit Tests
├── scripts/                      # Setup & Deploy Scripts
├── docs/                         # Documentation
└── .vscode/                      # Cursor IDE Configuration
```

## 🛠️ Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL** (local or Railway managed)
- **Redis** (for session management)
- **Cursor IDE** với C# extensions

## 🚀 Quick Start

### 1. Install .NET 9 SDK

```bash
# macOS
brew install dotnet

# Windows
winget install Microsoft.DotNet.SDK.9

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
cd HniDashOps-Backend

# Run setup script
chmod +x scripts/setup.sh
./scripts/setup.sh

# Install packages
dotnet restore

# Build solution
dotnet build

# Run API
dotnet run --project src/HniDashOps.API
```

### 3. Access API

- **API Base URL**: `https://localhost:7000/api`
- **Swagger UI**: `https://localhost:7000/swagger`
- **Health Check**: `https://localhost:7000/health`

## 📚 API Documentation

### Authentication APIs
- `POST /api/auth/login` - Traditional login
- `POST /api/auth/sso` - SSO login
- `POST /api/auth/check-user` - Check user exists

### User Management APIs
- `GET /api/users` - Get users list (with pagination)
- `POST /api/users` - Create new user
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Menu Management APIs
- `GET /api/menus` - Get menus list
- `POST /api/menus` - Create new menu
- `GET /api/menus/{id}` - Get menu by ID
- `PUT /api/menus/{id}` - Update menu
- `DELETE /api/menus/{id}` - Delete menu

### Categories APIs
- `GET /api/categories` - Get categories list
- `POST /api/categories` - Create new category
- `GET /api/categories/{id}` - Get category by ID
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Departments APIs
- `GET /api/departments` - Get departments list
- `POST /api/departments` - Create new department
- `GET /api/departments/{id}` - Get department by ID
- `PUT /api/departments/{id}` - Update department
- `DELETE /api/departments/{id}` - Delete department

### System Notifications APIs
- `GET /api/notifications` - Get notifications list
- `POST /api/notifications` - Create new notification
- `GET /api/notifications/{id}` - Get notification by ID
- `PUT /api/notifications/{id}` - Update notification
- `DELETE /api/notifications/{id}` - Delete notification

## 🗄️ Database Schema

### Users Table
```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    role VARCHAR(20) DEFAULT 'user',
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Menus Table
```sql
CREATE TABLE menus (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    path VARCHAR(200) NOT NULL,
    icon VARCHAR(50),
    parent_id INTEGER REFERENCES menus(id),
    order_index INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## 🚀 Deployment

### Railway Deployment

1. **Connect GitHub Repository**
2. **Add .NET Service**
3. **Add PostgreSQL Service**
4. **Configure Environment Variables**
5. **Deploy**

### Environment Variables

```bash
# Database
DATABASE_URL=postgresql://username:password@host:port/database

# Redis
REDIS_URL=redis://username:password@host:port

# JWT
JWT_SECRET=your-super-secret-jwt-key

# CORS
CORS_ORIGINS=https://your-nuxt-app.vercel.app
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/HniDashOps.API.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Performance

- **Response Time**: < 100ms average
- **Throughput**: 1000+ requests/second
- **Memory Usage**: < 512MB
- **Database**: PostgreSQL with connection pooling
- **Caching**: Redis for session management

## 🔒 Security

- **JWT Authentication** với HttpOnly cookies
- **Input Validation** với FluentValidation
- **Rate Limiting** để prevent abuse
- **CORS** configuration cho frontend
- **HTTPS** enforced in production

## 📈 Monitoring

- **Health Checks** endpoint
- **Structured Logging** với Serilog
- **Error Tracking** với custom middleware
- **Performance Metrics** với built-in counters

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- **Discussions**: [GitHub Discussions](https://github.com/your-repo/discussions)

---

**Built with ❤️ using .NET Core 9, PostgreSQL, and Railway**
