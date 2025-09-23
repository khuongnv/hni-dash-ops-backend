using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Interface for authentication and authorization services
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with username/email and password
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Authentication result with user information</returns>
        Task<AuthResult?> AuthenticateAsync(string usernameOrEmail, string password);

        /// <summary>
        /// Demo authentication - authenticates user with username only (no password required)
        /// WARNING: This method is for demonstration purposes only and should be removed in production
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Authentication result with user information</returns>
        Task<AuthResult?> AuthenticateDemoAsync(string username);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>Registration result</returns>
        Task<RegistrationResult> RegisterAsync(string username, string email, string password, string? firstName = null, string? lastName = null, string? phoneNumber = null);

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="currentPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>Password change result</returns>
        Task<PasswordChangeResult> ChangePasswordAsync(string username, string currentPassword, string newPassword);

        /// <summary>
        /// Generates a JWT token for a user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>JWT token</returns>
        string GenerateJwtToken(User user);

        /// <summary>
        /// Validates a JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Token validation result</returns>
        Task<TokenValidationResult> ValidateTokenAsync(string token);

        /// <summary>
        /// Gets user accessible menu IDs by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of accessible menu IDs</returns>
        Task<List<int>> GetUserMenuIdsAsync(int userId);

        /// <summary>
        /// Checks if user has access to specific menu
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="menuId">Menu ID</param>
        /// <returns>True if user has access to menu</returns>
        Task<bool> HasMenuAccessAsync(int userId, int menuId);
    }

    /// <summary>
    /// Authentication result
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public User? User { get; set; }
        public UserRole Role { get; set; }
        public List<int> MenuIds { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Registration result
    /// </summary>
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Password change result
    /// </summary>
    public class PasswordChangeResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Token validation result
    /// </summary>
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public User? User { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
