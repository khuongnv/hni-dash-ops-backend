# HniDashOps Backend

Hệ thống backend quản lý dashboard operations với kiến trúc Clean Architecture, sử dụng ASP.NET Core 9.0 và PostgreSQL.

## 📋 Mục lục

- [Tổng quan](#tổng-quan)
- [Kiến trúc hệ thống](#kiến-trúc-hệ-thống)
- [Quy tắc đặt tên Entity và Database](#quy-tắc-đặt-tên-entity-và-database)
- [Hệ thống phân quyền](#hệ-thống-phân-quyền)
- [Cài đặt và chạy dự án](#cài-đặt-và-chạy-dự-án)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Audit System](#audit-system)

## 🎯 Tổng quan

HniDashOps Backend là hệ thống quản lý dashboard với các tính năng chính:

- **Quản lý người dùng** với hệ thống role-based authorization
- **Quản lý menu** với cấu trúc phân cấp
- **Quản lý danh mục** (Categories) với cấu trúc tree
- **Quản lý phòng ban** (Departments) với cấu trúc phân cấp
- **Hệ thống thông báo** (System Notifications)
- **Audit tracking** đầy đủ cho tất cả thao tác dữ liệu

## 🏗️ Kiến trúc hệ thống

Dự án sử dụng Clean Architecture với 4 layers:

```
src/
├── HniDashOps.API/           # Presentation Layer (Controllers, DTOs)
├── HniDashOps.Core/          # Domain Layer (Entities, Services Interfaces)
├── HniDashOps.Infrastructure/# Infrastructure Layer (Data Access, Services Implementation)
└── HniDashOps.Shared/        # Shared Layer (Common utilities)
```

### 🔧 Công nghệ sử dụng

- **.NET 9.0** - Framework chính
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **JWT Authentication** - Xác thực

## 📝 Quy tắc đặt tên Entity và Database

### 🗂️ Quy tắc đặt tên Entity

#### 1. **Quan hệ 1->n (One-to-Many)**
- Entity bên trái đại diện cho **1** (One)
- Entity bên phải đại diện cho **n** (Many)
- Tên entity junction table: `{EntityBênTrái}{EntityBênPhải}`

**Ví dụ:**
```csharp
// User (1) -> Group (n) = UserGroup ❌ (SAI)
// Group (1) -> User (n) = GroupUser ✅ (ĐÚNG)

// Menu (1) -> Group (n) = MenuGroup ❌ (SAI) 
// Group (1) -> Menu (n) = GroupMenu ✅ (ĐÚNG)
```

#### 2. **Quy tắc đặt tên Table**
- Sử dụng **SNAKE_CASE** (chữ hoa, phân tách bằng dấu gạch dưới)
- Các từ có ý nghĩa được phân tách bằng `_`

**Ví dụ:**
```csharp
[Table("USERS")]           // Thay vì Users
[Table("GROUP_USERS")]     // Thay vì UserGroups  
[Table("GROUP_MENUS")]     // Thay vì MenuGroups
[Table("SYSTEM_NOTIFICATIONS")] // Thay vì SystemNotifications
```

#### 3. **Quy tắc đặt tên Column**
- Sử dụng **SNAKE_CASE** cho tất cả columns
- Các từ có ý nghĩa được phân tách bằng `_`

**Ví dụ:**
```csharp
[Column("USER_ID")]        // Thay vì UserId
[Column("ASSIGNED_AT")]    // Thay vì AssignedAt
[Column("CREATED_BY")]     // Thay vì CreatedBy
[Column("UPDATED_AT")]     // Thay vì UpdatedAt
[Column("IS_DELETED")]     // Thay vì IsDeleted
```

### 🗃️ Cấu trúc Entity chuẩn

#### **BaseEntity**
Tất cả entities đều kế thừa từ `BaseEntity` với các trường audit:

```csharp
public abstract class BaseEntity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    [Column("CREATED_BY")]
    public int? CreatedBy { get; set; }

    [Column("UPDATED_AT")]
    public DateTime? UpdatedAt { get; set; }

    [Column("UPDATED_BY")]
    public int? UpdatedBy { get; set; }

    [Column("IS_ACTIVE")]
    public bool IsActive { get; set; }

    [Column("IS_DELETED")]
    public bool IsDeleted { get; set; }

    [Column("DELETED_AT")]
    public DateTime? DeletedAt { get; set; }

    [Column("DELETED_BY")]
    public int? DeletedBy { get; set; }
}
```

#### **User Entity**
```csharp
[Table("USERS")]
public class User : BaseEntity
{
    [Column("USERNAME")]
    public string Username { get; set; }

    [Column("EMAIL")]
    public string Email { get; set; }

    [Column("PASSWORD_HASH")]
    public string PasswordHash { get; set; }

    [Column("FIRST_NAME")]
    public string FirstName { get; set; }

    [Column("LAST_NAME")]
    public string LastName { get; set; }

    [Column("PHONE_NUMBER")]
    public string? PhoneNumber { get; set; }

    [Column("EMAIL_CONFIRMED")]
    public bool EmailConfirmed { get; set; }

    [Column("LAST_LOGIN_AT")]
    public DateTime? LastLoginAt { get; set; }

    [Column("ROLE_ID")]
    public UserRole Role { get; set; }

    // Navigation properties
    public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
}
```

#### **Junction Tables**
```csharp
[Table("GROUP_USERS")]
public class GroupUser : BaseEntity
{
    [Column("USER_ID")]
    public int UserId { get; set; }

    [Column("GROUP_ID")]
    public int GroupId { get; set; }

    [Column("ASSIGNED_AT")]
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
```

## 🔐 Hệ thống phân quyền

### 🎭 User Roles

Hệ thống sử dụng enum `UserRole` thay vì entity riêng biệt:

```csharp
public enum UserRole
{
    SuperAdmin = 1,    // Quản trị viên cao cấp - bypass tất cả kiểm tra quyền
    SubAdmin = 2,      // Quản trị viên phụ
    Member = 3,        // Thành viên
    Guest = 4          // Khách
}
```

### 🛡️ Authorization System

#### **1. SuperAdmin Bypass**
- User có role `SuperAdmin` sẽ **bypass tất cả** kiểm tra quyền
- Có thể truy cập tất cả API endpoints (trừ authentication)

#### **2. Custom Authorization Attributes**

**AuthorizeSuperAdmin Attribute:**
```csharp
[AuthorizeSuperAdmin]
public class UsersController : ControllerBase
{
    // Chỉ SuperAdmin mới có thể truy cập
}
```

**AuthorizeMenus Attribute:**
```csharp
[AuthorizeMenus(new[] { 1, 2, 3 }, LogicType.AND)]
public async Task<IActionResult> GetMenus()
{
    // User phải có quyền truy cập TẤT CẢ menu ID 1, 2, 3
}

[AuthorizeMenus(new[] { 1, 2, 3 }, LogicType.OR)]
public async Task<IActionResult> GetMenus()
{
    // User chỉ cần có quyền truy cập ÍT NHẤT 1 trong menu ID 1, 2, 3
}
```

**AuthorizeResource Attribute:**
```csharp
[AuthorizeResource(AuthorizationType.Department, new[] { 1, 2 }, LogicType.AND)]
public async Task<IActionResult> GetDepartments()
{
    // User phải có quyền truy cập TẤT CẢ department ID 1, 2
}
```

#### **3. Authorization Logic**

**Cấu trúc phân quyền:**
```
User (1) -> GroupUser (n) -> Group (1) -> GroupMenu (n) -> Menu (1)
```

**Quy trình kiểm tra quyền:**
1. **Kiểm tra SuperAdmin**: Nếu user là SuperAdmin → cho phép truy cập
2. **Lấy danh sách Group**: Tìm tất cả Group mà user thuộc về
3. **Lấy danh sách Menu/Resource**: Từ các Group, lấy danh sách Menu/Resource được phép truy cập
4. **Kiểm tra Logic**: Áp dụng logic AND/OR để kiểm tra quyền truy cập

**Ví dụ kiểm tra quyền:**
```csharp
// User ID = 1 thuộc Group ID = 1, 2
// Group ID = 1 có Menu ID = 1, 2, 3
// Group ID = 2 có Menu ID = 2, 4, 5
// User 1 có quyền truy cập Menu: 1, 2, 3, 4, 5

// Kiểm tra [AuthorizeMenus([1, 2], AND)]
// User có Menu 1 ✓ và Menu 2 ✓ → Cho phép truy cập

// Kiểm tra [AuthorizeMenus([6, 7], AND)]  
// User không có Menu 6 ✗ → Từ chối truy cập
```

### 🔄 Authorization Handler

```csharp
public class ResourceAuthorizationHandler : AuthorizationHandler<ResourceAuthorizationRequirement>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceAuthorizationRequirement requirement)
    {
        // 1. Kiểm tra SuperAdmin
        if (IsSuperAdmin(context.User))
            context.Succeed(requirement);

        // 2. Lấy User ID từ claims
        var userId = GetCurrentUserId(context.User);
        
        // 3. Lấy danh sách quyền từ cache hoặc database
        var permissions = await GetUserPermissionsAsync(userId);
        
        // 4. Kiểm tra quyền theo logic AND/OR
        if (CheckPermissions(permissions, requirement))
            context.Succeed(requirement);
    }
}
```

## 🚀 Cài đặt và chạy dự án

### 📋 Yêu cầu hệ thống

- **.NET 9.0 SDK**
- **PostgreSQL 13+**
- **Visual Studio 2022** hoặc **VS Code**

### 🔧 Cài đặt

1. **Clone repository:**
```bash
git clone <repository-url>
cd hni-dash-ops-backend
```

2. **Restore packages:**
```bash
dotnet restore
```

3. **Cấu hình database:**
```bash
# Cập nhật connection string trong appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hni_dash_ops;Username=postgres;Password=your_password"
  }
}
```

4. **Chạy migrations:**
```bash
dotnet ef database update --project src/HniDashOps.Infrastructure --startup-project src/HniDashOps.API
```

5. **Seed dữ liệu mẫu:**
```bash
# Chạy API để seed dữ liệu
curl -X POST https://localhost:7001/api/seed/initialize
```

### 🏃‍♂️ Chạy development

```bash
# Chạy API
dotnet run --project src/HniDashOps.API

# API sẽ chạy tại: https://localhost:7001
```

## 📚 API Documentation

### 🔑 Authentication Endpoints

```http
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "superadmin",
  "password": "admin@123"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "username": "superadmin",
      "email": "admin@example.com",
      "role": 1,
      "firstName": "Super",
      "lastName": "Admin"
    }
  }
}
```

### 👥 User Management

```http
GET /api/users
Authorization: Bearer {token}

POST /api/users
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "password123",
  "firstName": "New",
  "lastName": "User",
  "roleId": 3
}
```

### 📋 Menu Management

```http
GET /api/menus
Authorization: Bearer {token}

POST /api/menus
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Dashboard",
  "href": "/dashboard",
  "icon": "fas fa-tachometer-alt",
  "order": 1,
  "isVisible": true
}
```

### 🏢 Department Management

```http
GET /api/departments
Authorization: Bearer {token}

POST /api/departments
Authorization: Bearer {token}
Content-Type: application/json

{
  "code": "IT",
  "name": "Information Technology",
  "note": "IT Department"
}
```

### 📂 Category Management

```http
GET /api/categories
Authorization: Bearer {token}

POST /api/categories
Authorization: Bearer {token}
Content-Type: application/json

{
  "code": "CAT001",
  "name": "Software",
  "parentId": null,
  "order": 1
}
```

### 🔔 System Notifications

```http
GET /api/notifications
Authorization: Bearer {token}

POST /api/notifications
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "System Maintenance",
  "message": "System will be under maintenance",
  "type": "warning",
  "priority": "high"
}
```

## 🗄️ Database Schema

### 📊 Entity Relationship Diagram

```
USERS (1) ──┐
            │
            ├── GROUP_USERS (n) ──┐
            │                     │
            └─────────────────────┼── GROUPS (1) ──┐
                                  │                │
                                  └────────────────┼── GROUP_MENUS (n) ──┐
                                                   │                   │
                                                   └───────────────────┼── MENUS (1)
                                                                       │
DEPARTMENTS (1) ──┐                                                  │
                  │                                                  │
                  ├── DEPARTMENTS (n) [self-reference]               │
                  │                                                  │
CATEGORIES (1) ───┼── CATEGORIES (n) [self-reference]               │
                  │                                                  │
SYSTEM_NOTIFICATIONS (1) ───────────────────────────────────────────┘
```

### 🔍 Indexes và Constraints

```sql
-- Primary Keys
ALTER TABLE USERS ADD CONSTRAINT PK_USERS PRIMARY KEY (ID);
ALTER TABLE GROUPS ADD CONSTRAINT PK_GROUPS PRIMARY KEY (ID);
ALTER TABLE MENUS ADD CONSTRAINT PK_MENUS PRIMARY KEY (ID);
ALTER TABLE GROUP_USERS ADD CONSTRAINT PK_GROUP_USERS PRIMARY KEY (USER_ID, GROUP_ID);
ALTER TABLE GROUP_MENUS ADD CONSTRAINT PK_GROUP_MENUS PRIMARY KEY (GROUP_ID, MENU_ID);

-- Unique Indexes
CREATE UNIQUE INDEX IX_USERS_USERNAME ON USERS (USERNAME) WHERE IS_DELETED = FALSE;
CREATE UNIQUE INDEX IX_USERS_EMAIL ON USERS (EMAIL) WHERE IS_DELETED = FALSE;
CREATE UNIQUE INDEX IX_CATEGORIES_CODE ON CATEGORIES (CODE) WHERE IS_DELETED = FALSE;
CREATE UNIQUE INDEX IX_CATEGORIES_MAP_ID ON CATEGORIES (MAP_ID) WHERE MAP_ID IS NOT NULL AND IS_DELETED = FALSE;
CREATE UNIQUE INDEX IX_DEPARTMENTS_CODE ON DEPARTMENTS (CODE) WHERE IS_DELETED = FALSE;
CREATE UNIQUE INDEX IX_DEPARTMENTS_MAP_ID ON DEPARTMENTS (MAP_ID) WHERE MAP_ID IS NOT NULL AND IS_DELETED = FALSE;

-- Foreign Key Constraints
ALTER TABLE GROUP_USERS ADD CONSTRAINT FK_GROUP_USERS_USER FOREIGN KEY (USER_ID) REFERENCES USERS (ID);
ALTER TABLE GROUP_USERS ADD CONSTRAINT FK_GROUP_USERS_GROUP FOREIGN KEY (GROUP_ID) REFERENCES GROUPS (ID);
ALTER TABLE GROUP_MENUS ADD CONSTRAINT FK_GROUP_MENUS_GROUP FOREIGN KEY (GROUP_ID) REFERENCES GROUPS (ID);
ALTER TABLE GROUP_MENUS ADD CONSTRAINT FK_GROUP_MENUS_MENU FOREIGN KEY (MENU_ID) REFERENCES MENUS (ID);
```

## 📝 Audit System

### 🔍 Audit Fields

Tất cả entities đều có các trường audit để tracking:

- **`CREATED_AT`** - Thời gian tạo
- **`CREATED_BY`** - User ID tạo record
- **`UPDATED_AT`** - Thời gian cập nhật cuối
- **`UPDATED_BY`** - User ID cập nhật cuối
- **`DELETED_AT`** - Thời gian xóa (soft delete)
- **`DELETED_BY`** - User ID xóa record
- **`IS_ACTIVE`** - Trạng thái active
- **`IS_DELETED`** - Trạng thái xóa

### 🔄 Auto Audit Population

Hệ thống tự động populate audit fields thông qua `BaseService`:

```csharp
public abstract class BaseService
{
    protected void SetCreatedAuditFields(BaseEntity entity)
    {
        var currentUserId = GetCurrentUserId();
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = currentUserId;
        entity.IsActive = true;
        entity.IsDeleted = false;
    }

    protected void SetUpdatedAuditFields(BaseEntity entity)
    {
        var currentUserId = GetCurrentUserId();
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = currentUserId;
    }

    protected void SetDeletedAuditFields(BaseEntity entity)
    {
        var currentUserId = GetCurrentUserId();
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = currentUserId;
        entity.IsDeleted = true;
        entity.IsActive = false;
    }
}
```

### 📊 Audit Query Examples

```sql
-- Tìm tất cả records được tạo bởi user ID = 1
SELECT * FROM USERS WHERE CREATED_BY = 1;

-- Tìm tất cả records được cập nhật trong 7 ngày qua
SELECT * FROM MENUS 
WHERE UPDATED_AT >= NOW() - INTERVAL '7 days'
ORDER BY UPDATED_AT DESC;

-- Tìm tất cả records đã bị xóa
SELECT * FROM CATEGORIES 
WHERE IS_DELETED = TRUE
ORDER BY DELETED_AT DESC;
```

## 🔧 Development Guidelines

### 📝 Coding Standards

1. **Naming Conventions:**
   - Classes: `PascalCase`
   - Methods: `PascalCase`
   - Properties: `PascalCase`
   - Variables: `camelCase`
   - Constants: `UPPER_CASE`
   - Database: `SNAKE_CASE`

2. **Entity Framework:**
   - Sử dụng `[Column]` attribute cho tất cả properties
   - Sử dụng `[Table]` attribute cho tất cả entities
   - Configure relationships trong `ApplicationDbContext`

3. **Services:**
   - Tất cả services kế thừa từ `BaseService`
   - Sử dụng async/await pattern
   - Implement proper error handling và logging

4. **Controllers:**
   - Sử dụng `[AuthorizeSuperAdmin]` cho tất cả endpoints (trừ auth)
   - Return consistent response format
   - Implement proper validation

### 🧪 Testing

```bash
# Chạy unit tests
dotnet test

# Chạy với coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 📦 Deployment

```bash
# Build for production
dotnet publish -c Release -o ./publish

# Chạy production
dotnet ./publish/HniDashOps.API.dll
```

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

- **Email**: support@example.com
- **Documentation**: [Wiki](https://github.com/your-repo/wiki)
- **Issues**: [GitHub Issues](https://github.com/your-repo/issues)

---

**HniDashOps Backend** - Built with ❤️ using .NET 9.0

nuxt-hni-dash-ops/
├── frontend/                 # Nuxt.js app (code hiện tại)
│   ├── components/
│   ├── composables/
│   ├── pages/
│   └── ...
├── backend/                  # .NET Core API
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
├── shared/                   # Shared types/interfaces
│   ├── types/
│   └── contracts/
├── docs/                     # Documentation
└── scripts/                  # Build/deploy scripts
