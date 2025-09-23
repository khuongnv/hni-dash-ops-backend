using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HniDashOps.Core.Services;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Authorization;
using HniDashOps.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AuthorizeSuperAdmin]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            
            var userResponses = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                EmailConfirmed = u.EmailConfirmed,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt,
                Role = u.RoleId.ToString()
            }).ToList();

            return Ok(new
            {
                Status = "Success",
                Count = userResponses.Count,
                Data = userResponses
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting users");
            return StatusCode(500, new 
            { 
                Status = "Error",
                Message = "An error occurred while retrieving users",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new 
                { 
                    Status = "Error",
                    Message = "User not found",
                    Timestamp = DateTime.UtcNow
                });
            }

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                Role = user.RoleId.ToString()
            };

            return Ok(new
            {
                Status = "Success",
                Data = userResponse
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user with ID: {UserId}", id);
            return StatusCode(500, new 
            { 
                Status = "Error",
                Message = "An error occurred while retrieving the user",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists
            if (await _userService.UsernameExistsAsync(request.Username))
            {
                return BadRequest(new 
                { 
                    Status = "Error",
                    Message = "Username already exists",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check if email already exists
            if (await _userService.EmailExistsAsync(request.Email))
            {
                return BadRequest(new 
                { 
                    Status = "Error",
                    Message = "Email already exists",
                    Timestamp = DateTime.UtcNow
                });
            }

            var user = await _userService.CreateUserAsync(
                request.Username,
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.RoleId);

            _logger.LogInformation("User created: {Username} with ID: {UserId}", user.Username, user.Id);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                Status = "Success",
                Message = "User created successfully",
                Data = new { Id = user.Id, Username = user.Username, Email = user.Email }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user: {Username}", request.Username);
            return StatusCode(500, new 
            { 
                Status = "Error",
                Message = "An error occurred while creating the user",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists (excluding current user)
            if (await _userService.UsernameExistsAsync(request.Username, id))
            {
                return BadRequest(new 
                { 
                    Status = "Error",
                    Message = "Username already exists",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check if email already exists (excluding current user)
            if (await _userService.EmailExistsAsync(request.Email, id))
            {
                return BadRequest(new 
                { 
                    Status = "Error",
                    Message = "Email already exists",
                    Timestamp = DateTime.UtcNow
                });
            }

            var user = await _userService.UpdateUserAsync(
                id,
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.IsActive,
                request.Password);

            if (user == null)
            {
                return NotFound(new 
                { 
                    Status = "Error",
                    Message = "User not found",
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("User updated: {Username} with ID: {UserId}", user.Username, user.Id);

            return Ok(new
            {
                Status = "Success",
                Message = "User updated successfully",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", id);
            return StatusCode(500, new 
            { 
                Status = "Error",
                Message = "An error occurred while updating the user",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await _userService.DeleteUserAsync(id);
            
            if (!success)
            {
                return NotFound(new 
                { 
                    Status = "Error",
                    Message = "User not found",
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("User deleted with ID: {UserId}", id);

            return Ok(new
            {
                Status = "Success",
                Message = "User deleted successfully",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
            return StatusCode(500, new 
            { 
                Status = "Error",
                Message = "An error occurred while deleting the user",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _userService.UpdateUserRoleAsync(id, request.RoleId);
            
            if (!success)
            {
                return BadRequest(new 
                { 
                    Status = "Error",
                    Message = "Failed to assign role. User or role not found, or user already has this role",
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("Role {RoleId} assigned to user {UserId}", request.RoleId, id);

            return Ok(new
            {
                Status = "Success",
                Message = "Role assigned successfully",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while assigning role {RoleId} to user {UserId}", request.RoleId, id);
            return StatusCode(500, new 
            { 
                Status = "Error",
                Message = "An error occurred while assigning the role",
                Timestamp = DateTime.UtcNow
            });
        }
    }

}


