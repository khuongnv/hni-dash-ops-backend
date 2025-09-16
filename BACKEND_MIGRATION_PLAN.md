# Kế Hoạch Chuyển Đổi Backend .NET Core 9 + Swagger

## 📋 Rà Soát API Endpoints Hiện Tại

### 🔍 Các API Endpoints Đang Sử Dụng:

#### 1. Authentication APIs:
```
POST /api/auth/login          # Đăng nhập truyền thống
POST /api/auth/login-sso      # Đăng nhập SSO
POST /api/auth/check-user     # Kiểm tra user tồn tại
```

#### 2. User Management APIs:
```
GET    /api/users             # Lấy danh sách users (có pagination, search, filter)
POST   /api/users             # Tạo user mới
GET    /api/users/[id]        # Lấy user theo ID
PUT    /api/users/[id]        # Cập nhật user
DELETE /api/users/[id]        # Xóa user
```

#### 3. Menu Management APIs:
```
GET    /api/menus             # Lấy danh sách menu
POST   /api/menus             # Tạo menu mới
GET    /api/menus/[id]        # Lấy menu theo ID
PUT    /api/menus/[id]        # Cập nhật menu
DELETE /api/menus/[id]        # Xóa menu
```

#### 4. System Notifications APIs:
```
GET    /api/system-notifications           # Lấy danh sách thông báo
POST   /api/system-notifications           # Tạo thông báo mới
GET    /api/system-notifications/[id]      # Lấy thông báo theo ID
PUT    /api/system-notifications/[id]      # Cập nhật thông báo
DELETE /api/system-notifications/[id]      # Xóa thông báo
```

#### 5. Categories APIs:
```
GET    /api/categories        # Lấy danh sách categories
POST   /api/categories        # Tạo category mới
GET    /api/categories/[id]   # Lấy category theo ID
PUT    /api/categories/[id]   # Cập nhật category
DELETE /api/categories/[id]   # Xóa category
```

#### 6. Departments APIs:
```
GET    /api/departments       # Lấy danh sách departments
POST   /api/departments       # Tạo department mới
GET    /api/departments/[id]  # Lấy department theo ID
PUT    /api/departments/[id]  # Cập nhật department
DELETE /api/departments/[id]  # Xóa department
```

#### 7. Git Log API:
```
GET    /api/git/log           # Lấy git commit history
```

---

## 🏗️ Thiết Kế Backend .NET Core 9 + Swagger

### 📁 Cấu Trúc Project Backend:

```
HniDashOps.Backend/
├── src/
│   ├── HniDashOps.API/                    # Web API Project
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── MenusController.cs
│   │   │   ├── CategoriesController.cs
│   │   │   ├── DepartmentsController.cs
│   │   │   ├── SystemNotificationsController.cs
│   │   │   └── GitController.cs
│   │   ├── Models/
│   │   │   ├── Requests/
│   │   │   ├── Responses/
│   │   │   └── Entities/
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── UserService.cs
│   │   │   └── ...
│   │   ├── Middleware/
│   │   │   ├── AuthenticationMiddleware.cs
│   │   │   └── ErrorHandlingMiddleware.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── HniDashOps.Core/                   # Business Logic
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── Models/
│   │   └── Exceptions/
│   ├── HniDashOps.Infrastructure/         # Data Access
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Configurations/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── Extensions/
│   └── HniDashOps.Shared/                 # Shared Models
│       ├── DTOs/
│       ├── Enums/
│       └── Constants/
├── tests/
│   ├── HniDashOps.API.Tests/
│   └── HniDashOps.Core.Tests/
└── docs/
    ├── swagger/
    └── api-documentation.md
```

### 🔧 Cấu Hình .NET Core 9:

#### Program.cs:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "HniDashOps API", 
        Version = "v1",
        Description = "API cho hệ thống quản lý HniDashOps"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
// ... other services

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNuxtApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://your-nuxt-app.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HniDashOps API v1");
        c.RoutePrefix = "api-docs";
    });
}

app.UseCors("AllowNuxtApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 🔄 Kế Hoạch Chuyển Đổi

### 📋 Phase 1: Setup Backend Infrastructure
1. **Tạo .NET Core 9 project** với Clean Architecture
2. **Cấu hình Swagger** với documentation đầy đủ
3. **Setup Entity Framework** với Oracle provider
4. **Cấu hình Redis** cho session management
5. **Setup CORS** để Nuxt app có thể gọi API

### 📋 Phase 2: Migrate Core APIs
1. **Authentication APIs** (login, logout, validate)
2. **User Management APIs** (CRUD operations)
3. **Menu Management APIs**
4. **Categories & Departments APIs**

### 📋 Phase 3: Advanced Features
1. **System Notifications APIs**
2. **Git Log API** (hoặc bỏ nếu không cần thiết)
3. **Pagination, Search, Filter** logic
4. **Error handling** và logging

### 📋 Phase 4: Frontend Integration
1. **Update Nuxt composables** để gọi .NET Core APIs
2. **Update API base URL** từ `/api/` sang `https://your-backend.com/api/`
3. **Handle authentication** với HttpOnly cookies
4. **Testing** và debugging

---

## 🗺️ API Mapping Strategy

### Current Nuxt API → .NET Core API:

| **Nuxt Endpoint** | **.NET Core Endpoint** | **Controller** | **Priority** |
|------------------|----------------------|----------------|--------------|
| `POST /api/auth/login` | `POST /api/auth/login` | `AuthController` | **High** |
| `POST /api/auth/login-sso` | `POST /api/auth/sso` | `AuthController` | **High** |
| `GET /api/users` | `GET /api/users` | `UsersController` | **High** |
| `POST /api/users` | `POST /api/users` | `UsersController` | **High** |
| `GET /api/menus` | `GET /api/menus` | `MenusController` | **Medium** |
| `GET /api/categories` | `GET /api/categories` | `CategoriesController` | **Medium** |
| `GET /api/departments` | `GET /api/departments` | `DepartmentsController` | **Medium** |
| `GET /api/system-notifications` | `GET /api/notifications` | `NotificationsController` | **Low** |
| `GET /api/git/log` | `GET /api/git/log` | `GitController` | **Low** |

---

## 💡 Recommendations

### 🚀 Immediate Actions:
1. **Start with Authentication APIs** - đây là core functionality
2. **Setup Swagger documentation** ngay từ đầu
3. **Use DTOs** thay vì expose entities trực tiếp
4. **Implement proper error handling** với consistent response format

### 🔒 Security Considerations:
1. **HttpOnly cookies** cho session management
2. **JWT tokens** cho API authentication
3. **Input validation** với FluentValidation
4. **Rate limiting** để prevent abuse

### 📊 Performance:
1. **Redis caching** cho frequently accessed data
2. **Pagination** cho large datasets
3. **Async/await** patterns
4. **Connection pooling** cho database

---

## 📝 Next Steps

1. **Tạo .NET Core 9 project** với cấu trúc Clean Architecture
2. **Implement Authentication APIs** đầu tiên
3. **Setup Swagger documentation**
4. **Test integration** với Nuxt frontend
5. **Gradually migrate** các APIs còn lại

---

## 🚀 Deployment Platform: Railway + PostgreSQL

### ✅ **Final Decision: Railway + PostgreSQL**

```yaml
✅ Backend: Railway (.NET Core 9)
✅ Database: PostgreSQL (Railway managed)
✅ Frontend: Vercel (Nuxt.js)
✅ Cost: $0 (1 tháng demo)
✅ Setup: Easy với GitHub integration
```

### 🎯 **Why Railway + PostgreSQL:**

#### **Railway Benefits:**
```yaml
✅ Zero Cost: $5 welcome credit (1-2 tháng)
✅ Easy Setup: GitHub integration
✅ Auto-deploy: Push to deploy
✅ Built-in Services: PostgreSQL, Redis
✅ Custom Domains: Professional URLs
✅ SSL Certificates: HTTPS by default
✅ Monitoring: Built-in metrics
✅ Scaling: Auto-scaling
```

#### **PostgreSQL Benefits:**
```yaml
✅ Open Source: Free và powerful
✅ Performance: Fast queries
✅ Features: Rich functionality
✅ Compatibility: Good với .NET Core
✅ Migration: Easy từ Oracle
✅ Support: Large community
```

### 📊 **Updated Cost Breakdown:**

| **Service** | **Cost** | **Duration** | **Features** |
|------------|----------|--------------|--------------|
| **Railway Backend** | $0 | 1 tháng | .NET Core API |
| **Railway PostgreSQL** | $0 | 1 tháng | Managed database |
| **Vercel Frontend** | $0 | Forever | Nuxt.js app |
| **Total** | **$0** | **1 tháng** | **Full stack** |

### 🔄 **Database Migration:**

#### **From Oracle → To PostgreSQL:**
```yaml
Current Tables:
- users
- menus  
- categories
- departments
- system_notifications

PostgreSQL Equivalent:
- users (same structure)
- menus (same structure)
- categories (same structure)
- departments (same structure)
- system_notifications (same structure)
```

**Migration sẽ đơn giản vì cấu trúc tương tự!**

### 🛠️ **Updated Program.cs Configuration:**

```csharp
// Database - Updated for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis - Railway managed
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// CORS - Updated for Railway
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNuxtApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000", 
            "https://your-nuxt-app.vercel.app",
            "https://your-railway-app.railway.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
```

### 🚀 **Railway Setup Steps:**

#### **Phase 1: Railway Project Setup**
```bash
1. Tạo Railway account
2. Connect GitHub repo
3. Add .NET Core service
4. Add PostgreSQL service
5. Configure environment variables
```

#### **Phase 2: .NET Core Development**
```bash
1. Create .NET Core 9 project
2. Setup Entity Framework với PostgreSQL
3. Implement API endpoints
4. Add Swagger documentation
5. Test locally
```

#### **Phase 3: Deploy & Integration**
```bash
1. Deploy to Railway
2. Update Nuxt API base URL
3. Test end-to-end
4. Demo ready!
```

### 📋 **Environment Variables (Railway):**

```bash
# Railway tự động tạo:
DATABASE_URL=postgresql://username:password@host:port/database
REDIS_URL=redis://username:password@host:port
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Custom variables:
JWT_SECRET=your-jwt-secret
CORS_ORIGINS=https://your-nuxt-app.vercel.app
```

### 🎯 **Timeline:**

```yaml
Week 1: Railway setup + .NET Core development
Week 2: API implementation + testing
Week 3: Frontend integration + testing
Week 4: Demo preparation + deployment
```

### 💡 **Benefits cho Demo:**

```yaml
✅ Zero Cost: Hoàn toàn miễn phí
✅ Easy Setup: GitHub integration
✅ Professional: Custom domains, SSL
✅ Scalable: Auto-scaling
✅ Reliable: 99.9% uptime
✅ Fast: Global CDN
✅ Secure: HTTPS by default
✅ Modern: Latest .NET Core 9
```

---

*Tài liệu này được tạo để lưu lại kế hoạch chuyển đổi từ Nuxt API sang .NET Core 9 backend với Swagger documentation, sử dụng Railway + PostgreSQL cho deployment.*
