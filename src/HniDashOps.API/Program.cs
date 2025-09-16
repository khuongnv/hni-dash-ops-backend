using Microsoft.EntityFrameworkCore;
using HniDashOps.Infrastructure.Data;
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
// TODO: Add your services here
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HniDashOps API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "HniDashOps API Documentation";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowNuxtApp");

app.UseAuthorization();

app.MapControllers();

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
