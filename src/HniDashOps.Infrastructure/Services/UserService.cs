using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Implementation of user management services
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.GroupUsers)
                    .ThenInclude(gu => gu.Group)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.GroupUsers)
                    .ThenInclude(gu => gu.Group)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.GroupUsers)
                    .ThenInclude(gu => gu.Group)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.GroupUsers)
                    .ThenInclude(gu => gu.Group)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string? firstName = null, string? lastName = null, string? phoneNumber = null, UserRole? roleId = null)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                RoleId = roleId ?? UserRole.Member, // Default to Member role
                IsActive = true,
                EmailConfirmed = false
            };

            // Set audit fields for creation
            SetCreatedAuditFields(user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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

            // Update password if provided
            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = HashPassword(password);
            }

            // Set audit fields for update
            SetUpdatedAuditFields(user);

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

            // Soft delete user
            SetDeletedAuditFields(user);

            // Also soft delete related group users
            var groupUsers = await _context.GroupUsers
                .Where(gu => gu.UserId == id)
                .ToListAsync();

            foreach (var gu in groupUsers)
            {
                SetDeletedAuditFields(gu);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User deleted: {Username} with ID: {UserId}", user.Username, user.Id);

            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, UserRole roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted)
            {
                return false;
            }

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {RoleId} updated for user {UserId}", roleId, userId);

            return true;
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
