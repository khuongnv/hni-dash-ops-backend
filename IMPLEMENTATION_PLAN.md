# 🚀 HniDashOps Backend Implementation Plan

## 📋 Tổng Quan Dự Án

### 🎯 **Mục Tiêu:**
Chuyển đổi từ Nuxt API sang .NET Core 9 Backend với Clean Architecture, deploy trên Railway + PostgreSQL.

### 📊 **Scope:**
- **7 nhóm APIs** cần migrate
- **29 endpoints** tổng cộng
- **Clean Architecture** với 4 layers
- **Swagger documentation** đầy đủ
- **Railway deployment** với PostgreSQL

## 🗓️ Timeline Implementation

### **Week 1: Core Infrastructure** (5 ngày)
```yaml
Day 1-2: Setup & Authentication APIs
Day 3-4: User Management APIs  
Day 5: Menu Management APIs
```

### **Week 2: Business Logic** (5 ngày)
```yaml
Day 1-2: Categories & Departments APIs
Day 3-4: System Notifications APIs
Day 5: Git Log API & Testing
```

### **Week 3: Integration & Testing** (5 ngày)
```yaml
Day 1-2: Frontend Integration
Day 3-4: End-to-end Testing
Day 5: Performance Optimization
```

### **Week 4: Deployment & Demo** (5 ngày)
```yaml
Day 1-2: Railway Deployment
Day 3-4: Production Testing
Day 5: Demo Preparation
```

## 🏗️ Detailed Implementation Steps

### **Phase 1: Setup Infrastructure** ⚡

#### **Step 1.1: Install .NET 9 SDK**
- [ ] macOS: `brew install dotnet`
- [ ] Windows: `winget install Microsoft.DotNet.SDK.9`
- [ ] Linux: Follow installation guide
- [ ] Verify: `dotnet --version`

#### **Step 1.2: Create Project Structure**
- [x] Create solution file
- [x] Create 4 projects (API, Core, Infrastructure, Shared)
- [x] Setup project references
- [x] Configure Cursor IDE settings

#### **Step 1.3: Setup Database**
- [ ] Install PostgreSQL (local or Railway)
- [ ] Configure Entity Framework
- [ ] Create initial migrations
- [ ] Setup connection strings

### **Phase 2: Core APIs Implementation** 🔐

#### **Step 2.1: Authentication APIs**
```csharp
// Priority: HIGH
POST /api/auth/login          // Traditional login
POST /api/auth/sso            // SSO login  
POST /api/auth/check-user     // Check user exists
```

**Implementation Tasks:**
- [ ] Create AuthController
- [ ] Implement JWT authentication
- [ ] Create login DTOs
- [ ] Setup password hashing
- [ ] Implement SSO logic
- [ ] Add authentication middleware

#### **Step 2.2: User Management APIs**
```csharp
// Priority: HIGH
GET    /api/users             // List users with pagination
POST   /api/users             // Create user
GET    /api/users/{id}        // Get user by ID
PUT    /api/users/{id}        // Update user
DELETE /api/users/{id}        // Delete user
```

**Implementation Tasks:**
- [ ] Create UsersController
- [ ] Implement CRUD operations
- [ ] Add pagination logic
- [ ] Create user DTOs
- [ ] Add validation
- [ ] Implement search/filter

### **Phase 3: Business Logic APIs** 📊

#### **Step 3.1: Menu Management APIs**
```csharp
// Priority: MEDIUM
GET    /api/menus             // List menus
POST   /api/menus             // Create menu
GET    /api/menus/{id}        // Get menu by ID
PUT    /api/menus/{id}        // Update menu
DELETE /api/menus/{id}        // Delete menu
```

**Implementation Tasks:**
- [ ] Create MenusController
- [ ] Implement menu hierarchy
- [ ] Add menu permissions
- [ ] Create menu DTOs
- [ ] Add menu ordering

#### **Step 3.2: Categories & Departments APIs**
```csharp
// Priority: MEDIUM
GET    /api/categories        // List categories
POST   /api/categories        // Create category
GET    /api/categories/{id}   // Get category by ID
PUT    /api/categories/{id}   // Update category
DELETE /api/categories/{id}   // Delete category

GET    /api/departments       // List departments
POST   /api/departments       // Create department
GET    /api/departments/{id}  // Get department by ID
PUT    /api/departments/{id}  // Update department
DELETE /api/departments/{id}  // Delete department
```

**Implementation Tasks:**
- [ ] Create CategoriesController
- [ ] Create DepartmentsController
- [ ] Implement CRUD operations
- [ ] Add relationships
- [ ] Create DTOs
- [ ] Add validation

### **Phase 4: Advanced Features** 🔔

#### **Step 4.1: System Notifications APIs**
```csharp
// Priority: LOW
GET    /api/notifications           // List notifications
POST   /api/notifications           // Create notification
GET    /api/notifications/{id}      // Get notification by ID
PUT    /api/notifications/{id}      // Update notification
DELETE /api/notifications/{id}      // Delete notification
```

**Implementation Tasks:**
- [ ] Create NotificationsController
- [ ] Implement notification system
- [ ] Add real-time notifications
- [ ] Create notification DTOs
- [ ] Add notification types

#### **Step 4.2: Git Log API**
```csharp
// Priority: LOW
GET    /api/git/log           // Get git commit history
```

**Implementation Tasks:**
- [ ] Create GitController
- [ ] Implement git log parsing
- [ ] Add git integration
- [ ] Create git DTOs

### **Phase 5: Documentation & Testing** 📚

#### **Step 5.1: Swagger Documentation**
- [ ] Configure Swagger UI
- [ ] Add XML comments
- [ ] Create API documentation
- [ ] Add examples
- [ ] Test all endpoints

#### **Step 5.2: Testing**
- [ ] Create unit tests
- [ ] Create integration tests
- [ ] Add API tests
- [ ] Performance testing
- [ ] Security testing

### **Phase 6: Deployment** 🚀

#### **Step 6.1: Railway Setup**
- [ ] Create Railway account
- [ ] Connect GitHub repository
- [ ] Add .NET service
- [ ] Add PostgreSQL service
- [ ] Configure environment variables

#### **Step 6.2: Production Deployment**
- [ ] Deploy to Railway
- [ ] Configure custom domain
- [ ] Setup SSL certificates
- [ ] Configure monitoring
- [ ] Performance optimization

## 📁 File Structure Implementation

### **Shared Layer (HniDashOps.Shared)**
```
HniDashOps.Shared/
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequest.cs
│   │   ├── LoginResponse.cs
│   │   └── SSORequest.cs
│   ├── Users/
│   │   ├── UserDto.cs
│   │   ├── CreateUserRequest.cs
│   │   └── UpdateUserRequest.cs
│   ├── Menus/
│   │   ├── MenuDto.cs
│   │   └── MenuRequest.cs
│   └── Common/
│       ├── PaginationRequest.cs
│       ├── PaginationResponse.cs
│       └── ApiResponse.cs
├── Enums/
│   ├── UserRole.cs
│   ├── MenuType.cs
│   └── NotificationType.cs
└── Constants/
    ├── ApiRoutes.cs
    └── ErrorMessages.cs
```

### **Core Layer (HniDashOps.Core)**
```
HniDashOps.Core/
├── Interfaces/
│   ├── IAuthService.cs
│   ├── IUserService.cs
│   ├── IMenuService.cs
│   └── INotificationService.cs
├── Services/
│   ├── AuthService.cs
│   ├── UserService.cs
│   ├── MenuService.cs
│   └── NotificationService.cs
├── Models/
│   ├── User.cs
│   ├── Menu.cs
│   ├── Category.cs
│   └── Department.cs
└── Exceptions/
    ├── NotFoundException.cs
    ├── ValidationException.cs
    └── UnauthorizedException.cs
```

### **Infrastructure Layer (HniDashOps.Infrastructure)**
```
HniDashOps.Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Configurations/
│       ├── UserConfiguration.cs
│       ├── MenuConfiguration.cs
│       └── CategoryConfiguration.cs
├── Repositories/
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── IMenuRepository.cs
│   └── MenuRepository.cs
├── Services/
│   ├── JwtService.cs
│   ├── PasswordService.cs
│   └── EmailService.cs
└── Extensions/
    ├── ServiceCollectionExtensions.cs
    └── ApplicationBuilderExtensions.cs
```

### **API Layer (HniDashOps.API)**
```
HniDashOps.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── MenusController.cs
│   ├── CategoriesController.cs
│   ├── DepartmentsController.cs
│   ├── NotificationsController.cs
│   └── GitController.cs
├── Middleware/
│   ├── AuthenticationMiddleware.cs
│   ├── ErrorHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
├── Filters/
│   ├── ValidationFilter.cs
│   └── AuthorizationFilter.cs
├── Program.cs
└── appsettings.json
```

## 🔧 Technical Implementation Details

### **Authentication & Authorization**
```csharp
// JWT Configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]))
        };
    });
```

### **Database Configuration**
```csharp
// PostgreSQL Configuration
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
        npgsqlOptions.CommandTimeout(30);
    }));
```

### **CORS Configuration**
```csharp
// CORS for Nuxt Frontend
services.AddCors(options =>
{
    options.AddPolicy("AllowNuxtApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://your-app.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### **Swagger Configuration**
```csharp
// Swagger Documentation
services.AddSwaggerGen(c =>
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
```

## 📊 Success Metrics

### **Performance Targets:**
- **Response Time**: < 100ms average
- **Throughput**: 1000+ requests/second
- **Memory Usage**: < 512MB
- **Database Queries**: < 50ms average

### **Quality Targets:**
- **Test Coverage**: > 80%
- **API Documentation**: 100% endpoints documented
- **Error Handling**: All exceptions handled gracefully
- **Security**: JWT authentication, input validation, CORS

### **Deployment Targets:**
- **Uptime**: 99.9%
- **Deployment Time**: < 5 minutes
- **Rollback Time**: < 2 minutes
- **Zero Downtime**: Blue-green deployment

## 🎯 Next Steps

### **Immediate Actions (Today):**
1. **Install .NET 9 SDK** trên máy development
2. **Run setup script**: `./scripts/setup.sh`
3. **Start with Authentication APIs** implementation
4. **Setup Swagger documentation**

### **This Week:**
1. **Complete Authentication APIs**
2. **Implement User Management APIs**
3. **Setup database với Entity Framework**
4. **Test APIs với Swagger UI**

### **Next Week:**
1. **Implement remaining APIs**
2. **Frontend integration**
3. **Railway deployment**
4. **End-to-end testing**

---

**Ready to Start Implementation! 🚀**

Bây giờ bạn có thể bắt đầu implement theo kế hoạch này. Tôi khuyến nghị bắt đầu với việc cài đặt .NET 9 SDK và chạy setup script.
