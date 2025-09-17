using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HniDashOps.Core.Services;
using HniDashOps.Core.Entities;
using HniDashOps.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.Controllers
{
    /// <summary>
    /// Controller for managing permissions in the system
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>List of all permissions</returns>
        [HttpGet]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionsAsync();
                
                var permissionResponses = permissions.Select(p => new PermissionResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Resource = p.Resource,
                    Action = p.Action,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = permissionResponses.Count,
                    Data = permissionResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting permissions");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving permissions",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets permissions grouped by resource
        /// </summary>
        /// <returns>Permissions grouped by resource</returns>
        [HttpGet("grouped")]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetPermissionsGrouped()
        {
            try
            {
                var groupedPermissions = await _permissionService.GetPermissionsGroupedByResourceAsync();
                
                var result = groupedPermissions.Select(g => new
                {
                    Resource = g.Key,
                    Permissions = g.Value.Select(p => new PermissionResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Resource = p.Resource,
                        Action = p.Action,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = result.Count,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting grouped permissions");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving grouped permissions",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a specific permission by ID
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>Permission details</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetPermission(int id)
        {
            try
            {
                var permission = await _permissionService.GetPermissionByIdAsync(id);
                
                if (permission == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Permission not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var permissionResponse = new PermissionResponse
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description,
                    Resource = permission.Resource,
                    Action = permission.Action,
                    IsActive = permission.IsActive,
                    CreatedAt = permission.CreatedAt,
                    UpdatedAt = permission.UpdatedAt
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = permissionResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting permission with ID: {PermissionId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the permission",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets permissions by resource
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <returns>Permissions for the specified resource</returns>
        [HttpGet("resource/{resource}")]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetPermissionsByResource(string resource)
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsByResourceAsync(resource);
                
                var permissionResponses = permissions.Select(p => new PermissionResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Resource = p.Resource,
                    Action = p.Action,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Resource = resource,
                    Count = permissionResponses.Count,
                    Data = permissionResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting permissions for resource: {Resource}", resource);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving permissions for the resource",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new permission
        /// </summary>
        /// <param name="request">Permission creation request</param>
        /// <returns>Created permission</returns>
        [HttpPost]
        [Authorize(Policy = "RequirePermissionsCreatePermission")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if permission name already exists
                if (await _permissionService.PermissionNameExistsAsync(request.Name))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Permission name already exists",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var permission = await _permissionService.CreatePermissionAsync(
                    request.Name, 
                    request.Description, 
                    request.Resource, 
                    request.Action);

                _logger.LogInformation("New permission created: {PermissionName} with ID: {PermissionId}", permission.Name, permission.Id);

                return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, new
                {
                    Status = "Success",
                    Message = "Permission created successfully",
                    Data = new
                    {
                        permission.Id,
                        permission.Name,
                        permission.Description,
                        permission.Resource,
                        permission.Action,
                        permission.IsActive,
                        permission.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating permission: {PermissionName}", request.Name);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while creating the permission",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates an existing permission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <param name="request">Permission update request</param>
        /// <returns>Updated permission</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequirePermissionsUpdatePermission")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if permission name already exists (excluding current permission)
                if (await _permissionService.PermissionNameExistsAsync(request.Name, id))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Permission name already exists",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var permission = await _permissionService.UpdatePermissionAsync(
                    id, 
                    request.Name, 
                    request.Description, 
                    request.Resource, 
                    request.Action, 
                    request.IsActive);
                
                if (permission == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Permission not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Permission updated: {PermissionName} with ID: {PermissionId}", permission.Name, permission.Id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Permission updated successfully",
                    Data = new
                    {
                        permission.Id,
                        permission.Name,
                        permission.Description,
                        permission.Resource,
                        permission.Action,
                        permission.IsActive,
                        permission.CreatedAt,
                        permission.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating permission with ID: {PermissionId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while updating the permission",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a permission (soft delete)
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequirePermissionsDeletePermission")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                // Check if permission is assigned to any roles
                if (await _permissionService.IsPermissionAssignedToRolesAsync(id))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Cannot delete permission that is assigned to roles",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var success = await _permissionService.DeletePermissionAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Permission not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Permission deleted with ID: {PermissionId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Permission deleted successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting permission with ID: {PermissionId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while deleting the permission",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets roles that have a specific permission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>List of roles with this permission</returns>
        [HttpGet("{id}/roles")]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetPermissionRoles(int id)
        {
            try
            {
                var permission = await _permissionService.GetPermissionByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Permission not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var rolePermissions = await _permissionService.GetPermissionRolesAsync(id);
                
                var roles = rolePermissions.Select(rp => new
                {
                    rp.Role.Id,
                    rp.Role.Name,
                    rp.Role.Description,
                    rp.Role.IsActive,
                    rp.AssignedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Permission = new { permission.Id, permission.Name, permission.Description, permission.Resource, permission.Action },
                    Count = roles.Count,
                    Data = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting roles for permission with ID: {PermissionId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving permission roles",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets all available resources
        /// </summary>
        /// <returns>List of unique resources</returns>
        [HttpGet("resources")]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetResources()
        {
            try
            {
                var resources = await _permissionService.GetResourcesAsync();

                return Ok(new
                {
                    Status = "Success",
                    Count = resources.Count,
                    Data = resources
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting resources");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving resources",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets all available actions
        /// </summary>
        /// <returns>List of unique actions</returns>
        [HttpGet("actions")]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        public async Task<IActionResult> GetActions()
        {
            try
            {
                var actions = await _permissionService.GetActionsAsync();

                return Ok(new
                {
                    Status = "Success",
                    Count = actions.Count,
                    Data = actions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting actions");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving actions",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

}
