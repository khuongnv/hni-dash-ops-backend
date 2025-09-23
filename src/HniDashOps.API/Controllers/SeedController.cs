using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HniDashOps.Infrastructure.Data;
using HniDashOps.Core.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HniDashOps.API.Controllers
{
    /// <summary>
    /// Controller for seeding initial data into the database
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeSuperAdmin]
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
                        Groups = await _context.Groups.CountAsync(),
                        Users = await _context.Users.CountAsync(),
                        GroupUsers = await _context.GroupUsers.CountAsync(),
                        GroupMenus = await _context.GroupMenus.CountAsync(),
                        Categories = await _context.Categories.CountAsync(),
                        Menus = await _context.Menus.CountAsync(),
                        Departments = await _context.Departments.CountAsync()
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
                    Groups = await _context.Groups.CountAsync(),
                    Users = await _context.Users.CountAsync(),
                    GroupUsers = await _context.GroupUsers.CountAsync(),
                    GroupMenus = await _context.GroupMenus.CountAsync(),
                    Categories = await _context.Categories.CountAsync(),
                    Menus = await _context.Menus.CountAsync(),
                    Departments = await _context.Departments.CountAsync(),
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
                _context.GroupUsers.RemoveRange(_context.GroupUsers);
                _context.GroupMenus.RemoveRange(_context.GroupMenus);
                _context.Users.RemoveRange(_context.Users);
                _context.Groups.RemoveRange(_context.Groups);

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
