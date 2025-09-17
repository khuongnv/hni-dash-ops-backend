using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Implementation of role management services
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(ApplicationDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);
        }

        public async Task<Role> CreateRoleAsync(string name, string? description = null)
        {
            var role = new Role
            {
                Name = name,
                Description = description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role created: {RoleName} with ID: {RoleId}", role.Name, role.Id);

            return role;
        }

        public async Task<Role?> UpdateRoleAsync(int id, string name, string? description = null, bool isActive = true)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || role.IsDeleted)
            {
                return null;
            }

            role.Name = name;
            role.Description = description;
            role.IsActive = isActive;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Role updated: {RoleName} with ID: {RoleId}", role.Name, role.Id);

            return role;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || role.IsDeleted)
            {
                return false;
            }

            // Check if role is assigned to any users
            var hasUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id && ur.IsActive && !ur.IsDeleted);
            if (hasUsers)
            {
                return false;
            }

            // Soft delete
            role.IsDeleted = true;
            role.UpdatedAt = DateTime.UtcNow;

            // Also soft delete related role permissions
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .ToListAsync();

            foreach (var rp in rolePermissions)
            {
                rp.IsDeleted = true;
                rp.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Role deleted: {RoleName} with ID: {RoleId}", role.Name, role.Id);

            return true;
        }

        public async Task<bool> AssignPermissionsAsync(int roleId, List<int> permissionIds)
        {
            // Check if role exists
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null || role.IsDeleted)
            {
                return false;
            }

            // Validate that all permission IDs exist
            var existingPermissions = await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync();

            if (existingPermissions.Count != permissionIds.Count)
            {
                return false;
            }

            // Remove existing role permissions for this role
            var existingRolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            foreach (var rp in existingRolePermissions)
            {
                rp.IsDeleted = true;
                rp.UpdatedAt = DateTime.UtcNow;
            }

            // Add new role permissions
            var rolePermissions = permissionIds.Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.RolePermissions.AddRange(rolePermissions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Permissions assigned to role: {RoleName} with ID: {RoleId}", role.Name, role.Id);

            return true;
        }

        public async Task<bool> RemovePermissionAsync(int roleId, int permissionId)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && !rp.IsDeleted);

            if (rolePermission == null)
            {
                return false;
            }

            // Soft delete
            rolePermission.IsDeleted = true;
            rolePermission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Permission {PermissionId} removed from role {RoleId}", permissionId, roleId);

            return true;
        }

        public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId && rp.IsActive && !rp.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<UserRole>> GetRoleUsersAsync(int roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId && ur.IsActive && !ur.IsDeleted && !ur.User.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> RoleNameExistsAsync(string name, int? excludeRoleId = null)
        {
            var query = _context.Roles.Where(r => r.Name == name && !r.IsDeleted);
            
            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsRoleAssignedToUsersAsync(int roleId)
        {
            return await _context.UserRoles.AnyAsync(ur => ur.RoleId == roleId && ur.IsActive && !ur.IsDeleted);
        }
    }
}
