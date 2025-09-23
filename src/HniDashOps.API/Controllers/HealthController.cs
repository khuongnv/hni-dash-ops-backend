using Microsoft.AspNetCore.Mvc;
using HniDashOps.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HniDashOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check database connection
            var canConnect = await _context.Database.CanConnectAsync();
            
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Database = canConnect ? "Connected" : "Disconnected",
                Version = "1.0.0"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }

    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        try
        {
            // Check if database is ready
            await _context.Database.OpenConnectionAsync();
            await _context.Database.CloseConnectionAsync();
            
            return Ok(new { Status = "Ready" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");
            return StatusCode(503, new { Status = "Not Ready", Error = ex.Message });
        }
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("database")]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            _logger.LogInformation("Testing database connection...");
            
            // Test basic connection
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                _logger.LogError("Database connection failed - Cannot connect to database");
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Message = "Database connection failed",
                    Timestamp = DateTime.UtcNow,
                    Database = "Cannot connect to database",
                    ConnectionString = _context.Database.GetConnectionString()
                });
            }

            // Test query execution
            var userCount = await _context.Users.CountAsync();
            var groupCount = await _context.Groups.CountAsync();
            var menuCount = await _context.Menus.CountAsync();

            _logger.LogInformation("Database connection test successful");

            return Ok(new
            {
                Status = "Healthy",
                Message = "Database connection successful",
                Timestamp = DateTime.UtcNow,
                Database = new
                {
                    Connection = "Connected",
                    Users = userCount,
                    Groups = groupCount,
                    Menus = menuCount,
                    Provider = _context.Database.ProviderName
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Message = "Database connection test failed",
                Timestamp = DateTime.UtcNow,
                Database = new
                {
                    Connection = "Failed",
                    Error = ex.Message,
                    Provider = _context.Database.ProviderName
                }
            });
        }
    }
}
