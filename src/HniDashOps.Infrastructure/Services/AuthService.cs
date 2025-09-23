using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Implementation of authentication and authorization services
    /// </summary>
    public class AuthService : BaseService, IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
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
                    .Include(u => u.GroupUsers)
                        .ThenInclude(gu => gu.Group)
                            .ThenInclude(g => g.GroupMenus)
                                .ThenInclude(gm => gm.Menu)
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

                // Get user menu access
                var menuIds = await GetUserMenuIdsAsync(user.Id);
                var token = GenerateJwtToken(user);

                _logger.LogInformation("User {Username} authenticated successfully", user.Username);

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    User = user,
                    Role = user.RoleId,
                    MenuIds = menuIds
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

        public async Task<AuthResult?> AuthenticateDemoAsync(string username)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.GroupUsers)
                        .ThenInclude(gu => gu.Group)
                            .ThenInclude(g => g.GroupMenus)
                                .ThenInclude(gm => gm.Menu)
                    .FirstOrDefaultAsync(u => 
                        u.Username == username 
                        && u.IsActive 
                        && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("Demo authentication failed for user: {Username}", username);
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "User not found or inactive"
                    };
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Get user menu IDs
                var menuIds = await GetUserMenuIdsAsync(user.Id);

                var token = GenerateJwtToken(user);

                _logger.LogInformation("User {Username} authenticated successfully via demo login", user.Username);

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    User = user,
                    Role = user.RoleId,
                    MenuIds = menuIds
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during demo authentication for user: {Username}", username);
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred during demo authentication"
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
                    RoleId = UserRole.Member, // Default role is Member
                    IsActive = true
                };

                // Set audit fields for creation
                SetCreatedAuditFields(user);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

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

            // Add role claim
            claims.Add(new Claim(ClaimTypes.Role, user.RoleId.ToString()));

            // Add menu access claims
            var menuIds = GetUserMenuIdsAsync(user.Id).Result;
            foreach (var menuId in menuIds)
            {
                claims.Add(new Claim("MenuAccess", menuId.ToString()));
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

        public async Task<List<int>> GetUserMenuIdsAsync(int userId)
        {
            return await _context.GroupUsers
                .Where(gu => gu.UserId == userId && gu.IsActive)
                .SelectMany(gu => gu.Group.GroupMenus
                    .Where(gm => gm.IsActive)
                    .Select(gm => gm.MenuId))
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> HasMenuAccessAsync(int userId, int menuId)
        {
            return await _context.GroupUsers
                .Where(gu => gu.UserId == userId && gu.IsActive)
                .SelectMany(gu => gu.Group.GroupMenus
                    .Where(gm => gm.IsActive && gm.MenuId == menuId))
                .AnyAsync();
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
