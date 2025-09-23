using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Interface for user management services
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User entity</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User entity</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User entity</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="roleId">Role to assign</param>
        /// <returns>Created user</returns>
        Task<User> CreateUserAsync(string username, string email, string password, string? firstName = null, string? lastName = null, string? phoneNumber = null, UserRole? roleId = null);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="isActive">Is active</param>
        /// <param name="password">New password (optional)</param>
        /// <returns>Updated user</returns>
        Task<User?> UpdateUserAsync(int id, string username, string email, string? firstName = null, string? lastName = null, string? phoneNumber = null, bool isActive = true, string? password = null);

        /// <summary>
        /// Deletes a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Updates user role
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleId">New role</param>
        /// <returns>True if updated successfully</returns>
        Task<bool> UpdateUserRoleAsync(int userId, UserRole roleId);

        /// <summary>
        /// Checks if username exists
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="excludeUserId">User ID to exclude from check</param>
        /// <returns>True if username exists</returns>
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);

        /// <summary>
        /// Checks if email exists
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="excludeUserId">User ID to exclude from check</param>
        /// <returns>True if email exists</returns>
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    }
}
