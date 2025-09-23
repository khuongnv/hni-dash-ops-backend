using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HniDashOps.Infrastructure.Data;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.Controllers
{
    /// <summary>
    /// Controller for authentication and authorization operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user with username/email and password
        /// </summary>
        /// <param name="request">Login request containing credentials</param>
        /// <returns>Authentication result with user information</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.AuthenticateAsync(request.UsernameOrEmail, request.Password);

                if (!result.Success)
                {
                    _logger.LogWarning("Login attempt failed for user: {UsernameOrEmail}", request.UsernameOrEmail);
                    return Unauthorized(new
                    {
                        Status = "Error",
                        Message = result.ErrorMessage,
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("User {Username} logged in successfully", result.User!.Username);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Login successful",
                    Timestamp = DateTime.UtcNow,
                    Token = result.Token,
                    User = new
                    {
                        result.User.Id,
                        result.User.Username,
                        result.User.Email,
                        result.User.FirstName,
                        result.User.LastName,
                        result.User.PhoneNumber,
                        result.User.EmailConfirmed,
                        result.User.LastLoginAt,
                        result.User.CreatedAt
                    },
                    Role = result.Role.ToString(),
                    MenuIds = result.MenuIds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user: {UsernameOrEmail}", request.UsernameOrEmail);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred during login",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">Registration request containing user information</param>
        /// <returns>Registration result</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = result.ErrorMessage,
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("New user registered: {Username}", result.User!.Username);

                return Ok(new
                {
                    Status = "Success",
                    Message = "User registered successfully",
                    Timestamp = DateTime.UtcNow,
                    User = new
                    {
                        result.User.Id,
                        result.User.Username,
                        result.User.Email,
                        result.User.FirstName,
                        result.User.LastName,
                        result.User.PhoneNumber,
                        result.User.EmailConfirmed,
                        result.User.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user: {Username}", request.Username);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred during registration",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Demo login endpoint - authenticates user with username only (no password required)
        /// WARNING: This endpoint is for demonstration purposes only and should be removed in production
        /// </summary>
        /// <param name="request">Demo login request containing only username</param>
        /// <returns>Authentication result with user information</returns>
        [HttpPost("demo-login")]
        public async Task<IActionResult> DemoLogin([FromBody] DemoLoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.AuthenticateDemoAsync(request.Username);

                if (!result.Success)
                {
                    _logger.LogWarning("Demo login attempt failed for user: {Username}", request.Username);
                    return Unauthorized(new
                    {
                        Status = "Error",
                        Message = result.ErrorMessage,
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("User {Username} logged in successfully via demo login", result.User!.Username);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Demo login successful",
                    Timestamp = DateTime.UtcNow,
                    Token = result.Token,
                    User = new
                    {
                        result.User.Id,
                        result.User.Username,
                        result.User.Email,
                        result.User.FirstName,
                        result.User.LastName,
                        result.User.PhoneNumber,
                        result.User.EmailConfirmed,
                        result.User.LastLoginAt,
                        result.User.CreatedAt
                    },
                    Role = result.Role.ToString(),
                    MenuIds = result.MenuIds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during demo login for user: {Username}", request.Username);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred during demo login",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="request">Password change request</param>
        /// <returns>Password change result</returns>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ChangePasswordAsync(request.Username, request.CurrentPassword, request.NewPassword);

                if (!result.Success)
                {
                    return result.ErrorMessage == "User not found" 
                        ? NotFound(new
                        {
                            Status = "Error",
                            Message = result.ErrorMessage,
                            Timestamp = DateTime.UtcNow
                        })
                        : Unauthorized(new
                        {
                            Status = "Error",
                            Message = result.ErrorMessage,
                            Timestamp = DateTime.UtcNow
                        });
                }

                _logger.LogInformation("Password changed for user: {Username}", request.Username);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Password changed successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password change for user: {Username}", request.Username);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred during password change",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

    public class LoginRequest
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class DemoLoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
