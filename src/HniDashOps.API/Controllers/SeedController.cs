using Microsoft.AspNetCore.Mvc;
using HniDashOps.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HniDashOps.API.Controllers
{
    /// <summary>
    /// Controller for seeding initial data into the database
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(ApplicationDbContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database with initial data (roles, permissions, users, etc.)
        /// </summary>
        /// <returns>Result of the seeding operation</returns>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                await SeedData.SeedAsync(_context);

                _logger.LogInformation("Database seeding completed successfully");

                return Ok(new
                {
                    Status = "Success",
                    Message = "Database seeded successfully",
                    Timestamp = DateTime.UtcNow,
                    Details = new
                    {
                        Roles = await _context.Roles.CountAsync(),
                        Permissions = await _context.Permissions.CountAsync(),
                        Users = await _context.Users.CountAsync(),
                        UserRoles = await _context.UserRoles.CountAsync(),
                        RolePermissions = await _context.RolePermissions.CountAsync()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database seeding");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Failed to seed database",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets the current seed data statistics
        /// </summary>
        /// <returns>Current database statistics</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetSeedStats()
        {
            try
            {
                var stats = new
                {
                    Roles = await _context.Roles.CountAsync(),
                    Permissions = await _context.Permissions.CountAsync(),
                    Users = await _context.Users.CountAsync(),
                    UserRoles = await _context.UserRoles.CountAsync(),
                    RolePermissions = await _context.RolePermissions.CountAsync(),
                    Timestamp = DateTime.UtcNow
                };

                return Ok(new
                {
                    Status = "Success",
                    Message = "Database statistics retrieved",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting seed stats");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Failed to get database statistics",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Clears all seed data from the database (DANGEROUS - for development only)
        /// </summary>
        /// <returns>Result of the clearing operation</returns>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearSeedData()
        {
            try
            {
                _logger.LogWarning("Clearing all seed data from database...");

                // Clear in correct order to respect foreign key constraints
                _context.UserRoles.RemoveRange(_context.UserRoles);
                _context.RolePermissions.RemoveRange(_context.RolePermissions);
                _context.Users.RemoveRange(_context.Users);
                _context.Permissions.RemoveRange(_context.Permissions);
                _context.Roles.RemoveRange(_context.Roles);

                await _context.SaveChangesAsync();

                _logger.LogWarning("All seed data cleared successfully");

                return Ok(new
                {
                    Status = "Success",
                    Message = "All seed data cleared successfully",
                    Timestamp = DateTime.UtcNow,
                    Warning = "This operation removed all seeded data from the database"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing seed data");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Failed to clear seed data",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
