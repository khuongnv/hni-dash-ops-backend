using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HniDashOps.Core.Services;
using HniDashOps.Core.Entities;
using HniDashOps.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.Controllers
{
    /// <summary>
    /// Controller for managing roles in the system
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        [HttpGet]
        [Authorize(Policy = "RequireRolesReadPermission")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                
                var roleResponses = roles.Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    Permissions = r.RolePermissions
                        .Where(rp => rp.IsActive && !rp.IsDeleted)
                        .Select(rp => new PermissionResponse
                        {
                            Id = rp.Permission.Id,
                            Name = rp.Permission.Name,
                            Description = rp.Permission.Description,
                            Resource = rp.Permission.Resource,
                            Action = rp.Permission.Action
                        }).ToList()
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = roleResponses.Count,
                    Data = roleResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting roles");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving roles",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a specific role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role details</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireRolesReadPermission")]
        public async Task<IActionResult> GetRole(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                
                if (role == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Role not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var roleResponse = new RoleResponse
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedAt = role.CreatedAt,
                    UpdatedAt = role.UpdatedAt,
                    Permissions = role.RolePermissions
                        .Where(rp => rp.IsActive && !rp.IsDeleted)
                        .Select(rp => new PermissionResponse
                        {
                            Id = rp.Permission.Id,
                            Name = rp.Permission.Name,
                            Description = rp.Permission.Description,
                            Resource = rp.Permission.Resource,
                            Action = rp.Permission.Action
                        }).ToList()
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = roleResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role with ID: {RoleId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the role",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="request">Role creation request</param>
        /// <returns>Created role</returns>
        [HttpPost]
        [Authorize(Policy = "RequireRolesCreatePermission")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if role name already exists
                if (await _roleService.RoleNameExistsAsync(request.Name))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Role name already exists",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var role = await _roleService.CreateRoleAsync(request.Name, request.Description);

                _logger.LogInformation("New role created: {RoleName} with ID: {RoleId}", role.Name, role.Id);

                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new
                {
                    Status = "Success",
                    Message = "Role created successfully",
                    Data = new
                    {
                        role.Id,
                        role.Name,
                        role.Description,
                        role.IsActive,
                        role.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role: {RoleName}", request.Name);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while creating the role",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="request">Role update request</param>
        /// <returns>Updated role</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireRolesUpdatePermission")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if role name already exists (excluding current role)
                if (await _roleService.RoleNameExistsAsync(request.Name, id))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Role name already exists",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var role = await _roleService.UpdateRoleAsync(id, request.Name, request.Description, request.IsActive);
                
                if (role == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Role not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Role updated: {RoleName} with ID: {RoleId}", role.Name, role.Id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Role updated successfully",
                    Data = new
                    {
                        role.Id,
                        role.Name,
                        role.Description,
                        role.IsActive,
                        role.CreatedAt,
                        role.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role with ID: {RoleId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while updating the role",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a role (soft delete)
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireRolesDeletePermission")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                // Check if role is assigned to any users
                if (await _roleService.IsRoleAssignedToUsersAsync(id))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Cannot delete role that is assigned to users",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var success = await _roleService.DeleteRoleAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Role not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Role deleted with ID: {RoleId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Role deleted successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role with ID: {RoleId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while deleting the role",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Assigns permissions to a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="request">Permission assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("{id}/permissions")]
        [Authorize(Policy = "RequireRolesUpdatePermission")]
        public async Task<IActionResult> AssignPermissions(int id, [FromBody] AssignPermissionsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _roleService.AssignPermissionsAsync(id, request.PermissionIds);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to assign permissions. Role not found or invalid permission IDs",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Permissions assigned to role with ID: {RoleId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Permissions assigned successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning permissions to role with ID: {RoleId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while assigning permissions",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets users assigned to a specific role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>List of users with this role</returns>
        [HttpGet("{id}/users")]
        [Authorize(Policy = "RequireRolesReadPermission")]
        public async Task<IActionResult> GetRoleUsers(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Role not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var userRoles = await _roleService.GetRoleUsersAsync(id);
                
                var users = userRoles.Select(ur => new
                {
                    ur.User.Id,
                    ur.User.Username,
                    ur.User.Email,
                    ur.User.FirstName,
                    ur.User.LastName,
                    ur.User.IsActive,
                    ur.AssignedAt,
                    ur.ExpiresAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Role = new { role.Id, role.Name, role.Description },
                    Count = users.Count,
                    Data = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users for role with ID: {RoleId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving role users",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

}
