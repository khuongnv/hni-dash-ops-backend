using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HniDashOps.Infrastructure.Data;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "HniDashOps API", 
        Version = "v1",
        Description = "API cho hệ thống quản lý HniDashOps với Supabase PostgreSQL"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database configuration - Supabase PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(30);
    });
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNuxtApp", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Default origins if not configured
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISystemNotificationService, SystemNotificationService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var issuer = jwtSettings["Issuer"] ?? "HniDashOps";
var audience = jwtSettings["Audience"] ?? "HniDashOpsUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    // Add custom authorization policies
    options.AddPolicy("RequireUsersReadPermission", policy =>
        policy.RequireClaim("Permission", "users.read"));
    
    options.AddPolicy("RequireUsersCreatePermission", policy =>
        policy.RequireClaim("Permission", "users.create"));
    
    options.AddPolicy("RequireUsersUpdatePermission", policy =>
        policy.RequireClaim("Permission", "users.update"));
    
    options.AddPolicy("RequireUsersDeletePermission", policy =>
        policy.RequireClaim("Permission", "users.delete"));
    
    options.AddPolicy("RequireRolesReadPermission", policy =>
        policy.RequireClaim("Permission", "roles.read"));
    
    options.AddPolicy("RequireRolesCreatePermission", policy =>
        policy.RequireClaim("Permission", "roles.create"));
    
    options.AddPolicy("RequireRolesUpdatePermission", policy =>
        policy.RequireClaim("Permission", "roles.update"));
    
    options.AddPolicy("RequireRolesDeletePermission", policy =>
        policy.RequireClaim("Permission", "roles.delete"));
    
    options.AddPolicy("RequirePermissionsReadPermission", policy =>
        policy.RequireClaim("Permission", "permissions.read"));
    
    options.AddPolicy("RequirePermissionsCreatePermission", policy =>
        policy.RequireClaim("Permission", "permissions.create"));
    
    options.AddPolicy("RequirePermissionsUpdatePermission", policy =>
        policy.RequireClaim("Permission", "permissions.update"));
    
    options.AddPolicy("RequirePermissionsDeletePermission", policy =>
        policy.RequireClaim("Permission", "permissions.delete"));
    
    options.AddPolicy("RequireDepartmentsReadPermission", policy =>
        policy.RequireClaim("Permission", "departments.read"));
    
    options.AddPolicy("RequireDepartmentsCreatePermission", policy =>
        policy.RequireClaim("Permission", "departments.create"));
    
    options.AddPolicy("RequireDepartmentsUpdatePermission", policy =>
        policy.RequireClaim("Permission", "departments.update"));
    
    options.AddPolicy("RequireDepartmentsDeletePermission", policy =>
        policy.RequireClaim("Permission", "departments.delete"));
    
    options.AddPolicy("RequireMenusReadPermission", policy =>
        policy.RequireClaim("Permission", "menus.read"));
    
    options.AddPolicy("RequireMenusCreatePermission", policy =>
        policy.RequireClaim("Permission", "menus.create"));
    
    options.AddPolicy("RequireMenusUpdatePermission", policy =>
        policy.RequireClaim("Permission", "menus.update"));
    
    options.AddPolicy("RequireMenusDeletePermission", policy =>
        policy.RequireClaim("Permission", "menus.delete"));
    
    options.AddPolicy("RequireCategoriesReadPermission", policy =>
        policy.RequireClaim("Permission", "categories.read"));
    
    options.AddPolicy("RequireCategoriesCreatePermission", policy =>
        policy.RequireClaim("Permission", "categories.create"));
    
    options.AddPolicy("RequireCategoriesUpdatePermission", policy =>
        policy.RequireClaim("Permission", "categories.update"));
    
    options.AddPolicy("RequireCategoriesDeletePermission", policy =>
        policy.RequireClaim("Permission", "categories.delete"));

    // SystemNotification policies
    options.AddPolicy("RequireNotificationsReadPermission", policy =>
        policy.RequireClaim("Permission", "notifications.read"));
    
    options.AddPolicy("RequireNotificationsCreatePermission", policy =>
        policy.RequireClaim("Permission", "notifications.create"));
    
    options.AddPolicy("RequireNotificationsUpdatePermission", policy =>
        policy.RequireClaim("Permission", "notifications.update"));
    
    options.AddPolicy("RequireNotificationsDeletePermission", policy =>
        policy.RequireClaim("Permission", "notifications.delete"));
    
    options.AddPolicy("RequireSystemAdminPermission", policy =>
        policy.RequireClaim("Permission", "system.admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger for both Development and Production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HniDashOps API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "HniDashOps API Documentation";
});

app.UseHttpsRedirection();

// Serve static files for frontend
app.UseStaticFiles();

app.UseCors("AllowNuxtApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { 
    Status = "Healthy", 
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Database = "Supabase PostgreSQL"
}));

// Database migration endpoint (development only)
if (app.Environment.IsDevelopment())
{
    app.MapPost("/migrate", async (ApplicationDbContext context) =>
    {
        try
        {
            await context.Database.MigrateAsync();
            return Results.Ok(new { Message = "Database migration completed successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Migration failed: {ex.Message}");
        }
    });
}

app.Run();
