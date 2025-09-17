using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Interface for role management services
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <returns>List of roles</returns>
        Task<List<Role>> GetAllRolesAsync();

        /// <summary>
        /// Gets a role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role entity</returns>
        Task<Role?> GetRoleByIdAsync(int id);

        /// <summary>
        /// Gets a role by name
        /// </summary>
        /// <param name="name">Role name</param>
        /// <returns>Role entity</returns>
        Task<Role?> GetRoleByNameAsync(string name);

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="name">Role name</param>
        /// <param name="description">Role description</param>
        /// <returns>Created role</returns>
        Task<Role> CreateRoleAsync(string name, string? description = null);

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="name">Role name</param>
        /// <param name="description">Role description</param>
        /// <param name="isActive">Is active</param>
        /// <returns>Updated role</returns>
        Task<Role?> UpdateRoleAsync(int id, string name, string? description = null, bool isActive = true);

        /// <summary>
        /// Deletes a role (soft delete)
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteRoleAsync(int id);

        /// <summary>
        /// Assigns permissions to a role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="permissionIds">Permission IDs</param>
        /// <returns>True if assigned successfully</returns>
        Task<bool> AssignPermissionsAsync(int roleId, List<int> permissionIds);

        /// <summary>
        /// Removes a permission from a role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="permissionId">Permission ID</param>
        /// <returns>True if removed successfully</returns>
        Task<bool> RemovePermissionAsync(int roleId, int permissionId);

        /// <summary>
        /// Gets role permissions
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List of role permissions</returns>
        Task<List<RolePermission>> GetRolePermissionsAsync(int roleId);

        /// <summary>
        /// Gets users assigned to a role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List of user roles</returns>
        Task<List<UserRole>> GetRoleUsersAsync(int roleId);

        /// <summary>
        /// Checks if role name exists
        /// </summary>
        /// <param name="name">Role name</param>
        /// <param name="excludeRoleId">Role ID to exclude from check</param>
        /// <returns>True if role name exists</returns>
        Task<bool> RoleNameExistsAsync(string name, int? excludeRoleId = null);

        /// <summary>
        /// Checks if role is assigned to any users
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>True if role is assigned to users</returns>
        Task<bool> IsRoleAssignedToUsersAsync(int roleId);
    }
}
