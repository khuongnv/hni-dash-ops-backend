using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Implementation of permission management services
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(ApplicationDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Permission?> GetPermissionByIdAsync(int id)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Permission?> GetPermissionByNameAsync(string name)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == name && !p.IsDeleted);
        }

        public async Task<List<Permission>> GetPermissionsByResourceAsync(string resource)
        {
            return await _context.Permissions
                .Where(p => p.Resource == resource && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<Permission>>> GetPermissionsGroupedByResourceAsync()
        {
            var permissions = await _context.Permissions
                .Where(p => !p.IsDeleted && !string.IsNullOrEmpty(p.Resource))
                .ToListAsync();

            return permissions
                .GroupBy(p => p.Resource!)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<Permission> CreatePermissionAsync(string name, string? description = null, string? resource = null, string? action = null)
        {
            var permission = new Permission
            {
                Name = name,
                Description = description,
                Resource = resource,
                Action = action,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Permission created: {PermissionName} with ID: {PermissionId}", permission.Name, permission.Id);

            return permission;
        }

        public async Task<Permission?> UpdatePermissionAsync(int id, string name, string? description = null, string? resource = null, string? action = null, bool isActive = true)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null || permission.IsDeleted)
            {
                return null;
            }

            permission.Name = name;
            permission.Description = description;
            permission.Resource = resource;
            permission.Action = action;
            permission.IsActive = isActive;
            permission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Permission updated: {PermissionName} with ID: {PermissionId}", permission.Name, permission.Id);

            return permission;
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null || permission.IsDeleted)
            {
                return false;
            }

            // Check if permission is assigned to any roles
            var hasRoles = await _context.RolePermissions.AnyAsync(rp => rp.PermissionId == id && rp.IsActive && !rp.IsDeleted);
            if (hasRoles)
            {
                return false;
            }

            // Soft delete
            permission.IsDeleted = true;
            permission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Permission deleted: {PermissionName} with ID: {PermissionId}", permission.Name, permission.Id);

            return true;
        }

        public async Task<List<RolePermission>> GetPermissionRolesAsync(int permissionId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Where(rp => rp.PermissionId == permissionId && rp.IsActive && !rp.IsDeleted && !rp.Role.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<string>> GetResourcesAsync()
        {
            return await _context.Permissions
                .Where(p => !p.IsDeleted && !string.IsNullOrEmpty(p.Resource))
                .Select(p => p.Resource!)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();
        }

        public async Task<List<string>> GetActionsAsync()
        {
            return await _context.Permissions
                .Where(p => !p.IsDeleted && !string.IsNullOrEmpty(p.Action))
                .Select(p => p.Action!)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<bool> PermissionNameExistsAsync(string name, int? excludePermissionId = null)
        {
            var query = _context.Permissions.Where(p => p.Name == name && !p.IsDeleted);
            
            if (excludePermissionId.HasValue)
            {
                query = query.Where(p => p.Id != excludePermissionId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsPermissionAssignedToRolesAsync(int permissionId)
        {
            return await _context.RolePermissions.AnyAsync(rp => rp.PermissionId == permissionId && rp.IsActive && !rp.IsDeleted);
        }
    }
}
