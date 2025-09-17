using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Implementation of authentication and authorization services
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResult?> AuthenticateAsync(string usernameOrEmail, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => 
                        (u.Username == usernameOrEmail || u.Email == usernameOrEmail) 
                        && u.IsActive 
                        && !u.IsDeleted);

                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    _logger.LogWarning("Authentication failed for user: {UsernameOrEmail}", usernameOrEmail);
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid username/email or password"
                    };
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Get user roles and permissions
                var roles = user.UserRoles
                    .Where(ur => ur.IsActive && !ur.IsDeleted)
                    .Select(ur => ur.Role)
                    .ToList();

                var permissions = user.UserRoles
                    .Where(ur => ur.IsActive && !ur.IsDeleted)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => rp.IsActive && !rp.IsDeleted)
                    .Select(rp => rp.Permission)
                    .Distinct()
                    .ToList();

                var token = GenerateJwtToken(user);

                _logger.LogInformation("User {Username} authenticated successfully", user.Username);

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    User = user,
                    Roles = roles,
                    Permissions = permissions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during authentication for user: {UsernameOrEmail}", usernameOrEmail);
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred during authentication"
                };
            }
        }

        public async Task<RegistrationResult> RegisterAsync(string username, string email, string password, string? firstName = null, string? lastName = null, string? phoneNumber = null)
        {
            try
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        ErrorMessage = "Username already exists"
                    };
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        ErrorMessage = "Email already exists"
                    };
                }

                // Create new user
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    EmailConfirmed = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign default role (User)
                var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (defaultRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = defaultRole.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("New user registered: {Username}", user.Username);

                return new RegistrationResult
                {
                    Success = true,
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user: {Username}", username);
                return new RegistrationResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred during registration"
                };
            }
        }

        public async Task<PasswordChangeResult> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive && !u.IsDeleted);

                if (user == null)
                {
                    return new PasswordChangeResult
                    {
                        Success = false,
                        ErrorMessage = "User not found"
                    };
                }

                // Verify current password
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return new PasswordChangeResult
                    {
                        Success = false,
                        ErrorMessage = "Current password is incorrect"
                    };
                }

                // Update password
                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Password changed for user: {Username}", user.Username);

                return new PasswordChangeResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password change for user: {Username}", username);
                return new PasswordChangeResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred during password change"
                };
            }
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "HniDashOps";
            var audience = jwtSettings["Audience"] ?? "HniDashOpsUsers";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new("FirstName", user.FirstName ?? ""),
                new("LastName", user.LastName ?? ""),
                new("PhoneNumber", user.PhoneNumber ?? ""),
                new("EmailConfirmed", user.EmailConfirmed.ToString())
            };

            // Add role claims
            var userRoles = _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id && ur.IsActive && !ur.IsDeleted)
                .Select(ur => ur.Role.Name)
                .ToList();

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permission claims
            var permissions = GetUserPermissionsAsync(user.Id).Result;
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission.Name));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<HniDashOps.Core.Services.TokenValidationResult> ValidateTokenAsync(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
                var issuer = jwtSettings["Issuer"] ?? "HniDashOps";
                var audience = jwtSettings["Audience"] ?? "HniDashOpsUsers";

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return new HniDashOps.Core.Services.TokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid token claims"
                    };
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null || !user.IsActive || user.IsDeleted)
                {
                    return new HniDashOps.Core.Services.TokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "User not found or inactive"
                    };
                }

                return new HniDashOps.Core.Services.TokenValidationResult
                {
                    IsValid = true,
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                return new HniDashOps.Core.Services.TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token validation failed"
                };
            }
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == userId && ur.IsActive && !ur.IsDeleted)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.IsActive && !rp.IsDeleted)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == userId && ur.IsActive && !ur.IsDeleted)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.IsActive && !rp.IsDeleted)
                .AnyAsync(rp => rp.Permission.Name == permissionName);
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
