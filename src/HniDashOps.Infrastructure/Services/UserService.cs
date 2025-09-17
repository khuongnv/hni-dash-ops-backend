using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Implementation of user management services
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string? firstName = null, string? lastName = null, string? phoneNumber = null, List<int>? roleIds = null)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign roles if provided
            if (roleIds != null && roleIds.Any())
            {
                foreach (var roleId in roleIds)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserRoles.Add(userRole);
                }
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("User created: {Username} with ID: {UserId}", user.Username, user.Id);

            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, string username, string email, string? firstName = null, string? lastName = null, string? phoneNumber = null, bool isActive = true, string? password = null)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
            {
                return null;
            }

            user.Username = username;
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update password if provided
            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = HashPassword(password);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User updated: {Username} with ID: {UserId}", user.Username, user.Id);

            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
            {
                return false;
            }

            // Soft delete
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            // Also soft delete related user roles
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .ToListAsync();

            foreach (var ur in userRoles)
            {
                ur.IsDeleted = true;
                ur.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User deleted: {Username} with ID: {UserId}", user.Username, user.Id);

            return true;
        }

        public async Task<bool> AssignRoleAsync(int userId, int roleId, DateTime? expiresAt = null)
        {
            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted)
            {
                return false;
            }

            // Check if role exists
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null || role.IsDeleted)
            {
                return false;
            }

            // Check if user already has this role
            if (await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted))
            {
                return false;
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {RoleId} assigned to user {UserId}", roleId, userId);

            return true;
        }

        public async Task<bool> RemoveRoleAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);

            if (userRole == null)
            {
                return false;
            }

            // Soft delete
            userRole.IsDeleted = true;
            userRole.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {RoleId} removed from user {UserId}", roleId, userId);

            return true;
        }

        public async Task<List<UserRole>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.IsActive && !ur.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Username == username && !u.IsDeleted);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email && !u.IsDeleted);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
