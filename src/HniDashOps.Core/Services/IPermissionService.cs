using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Interface for permission management services
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>List of permissions</returns>
        Task<List<Permission>> GetAllPermissionsAsync();

        /// <summary>
        /// Gets a permission by ID
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>Permission entity</returns>
        Task<Permission?> GetPermissionByIdAsync(int id);

        /// <summary>
        /// Gets a permission by name
        /// </summary>
        /// <param name="name">Permission name</param>
        /// <returns>Permission entity</returns>
        Task<Permission?> GetPermissionByNameAsync(string name);

        /// <summary>
        /// Gets permissions by resource
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <returns>List of permissions for the resource</returns>
        Task<List<Permission>> GetPermissionsByResourceAsync(string resource);

        /// <summary>
        /// Gets permissions grouped by resource
        /// </summary>
        /// <returns>Dictionary of resource and permissions</returns>
        Task<Dictionary<string, List<Permission>>> GetPermissionsGroupedByResourceAsync();

        /// <summary>
        /// Creates a new permission
        /// </summary>
        /// <param name="name">Permission name</param>
        /// <param name="description">Permission description</param>
        /// <param name="resource">Resource name</param>
        /// <param name="action">Action name</param>
        /// <returns>Created permission</returns>
        Task<Permission> CreatePermissionAsync(string name, string? description = null, string? resource = null, string? action = null);

        /// <summary>
        /// Updates an existing permission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <param name="name">Permission name</param>
        /// <param name="description">Permission description</param>
        /// <param name="resource">Resource name</param>
        /// <param name="action">Action name</param>
        /// <param name="isActive">Is active</param>
        /// <returns>Updated permission</returns>
        Task<Permission?> UpdatePermissionAsync(int id, string name, string? description = null, string? resource = null, string? action = null, bool isActive = true);

        /// <summary>
        /// Deletes a permission (soft delete)
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeletePermissionAsync(int id);

        /// <summary>
        /// Gets roles that have a specific permission
        /// </summary>
        /// <param name="permissionId">Permission ID</param>
        /// <returns>List of role permissions</returns>
        Task<List<RolePermission>> GetPermissionRolesAsync(int permissionId);

        /// <summary>
        /// Gets all unique resources
        /// </summary>
        /// <returns>List of unique resources</returns>
        Task<List<string>> GetResourcesAsync();

        /// <summary>
        /// Gets all unique actions
        /// </summary>
        /// <returns>List of unique actions</returns>
        Task<List<string>> GetActionsAsync();

        /// <summary>
        /// Checks if permission name exists
        /// </summary>
        /// <param name="name">Permission name</param>
        /// <param name="excludePermissionId">Permission ID to exclude from check</param>
        /// <returns>True if permission name exists</returns>
        Task<bool> PermissionNameExistsAsync(string name, int? excludePermissionId = null);

        /// <summary>
        /// Checks if permission is assigned to any roles
        /// </summary>
        /// <param name="permissionId">Permission ID</param>
        /// <returns>True if permission is assigned to roles</returns>
        Task<bool> IsPermissionAssignedToRolesAsync(int permissionId);
    }
}
