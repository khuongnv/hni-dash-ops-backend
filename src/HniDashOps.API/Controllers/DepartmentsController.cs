using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HniDashOps.Core.Services;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Authorization;
using HniDashOps.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.Controllers
{
    /// <summary>
    /// Controller for managing departments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeSuperAdmin]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all departments
        /// </summary>
        /// <returns>List of departments</returns>
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                
                var departmentResponses = departments.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    ParentId = d.ParentId,
                    ParentName = d.Parent?.Name,
                    MapId = d.MapId,
                    Level = d.Level,
                    Note = d.Note,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    UserCount = d.Users.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = departmentResponses.Count,
                    Data = departmentResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting departments");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving departments",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a specific department by ID
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);
                
                if (department == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Department not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var departmentResponse = new DepartmentResponse
                {
                    Id = department.Id,
                    Code = department.Code,
                    Name = department.Name,
                    ParentId = department.ParentId,
                    ParentName = department.Parent?.Name,
                    MapId = department.MapId,
                    Level = department.Level,
                    Note = department.Note,
                    IsActive = department.IsActive,
                    CreatedAt = department.CreatedAt,
                    UpdatedAt = department.UpdatedAt,
                    UserCount = department.Users.Count,
                    Children = department.Children.Select(c => new DepartmentResponse
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        ParentId = c.ParentId,
                        MapId = c.MapId,
                        Level = c.Level,
                        Note = c.Note,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }).ToList()
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = departmentResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department with ID: {DepartmentId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets department by code
        /// </summary>
        /// <param name="code">Department code</param>
        /// <returns>Department details</returns>
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetDepartmentByCode(string code)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByCodeAsync(code);
                
                if (department == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Department not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var departmentResponse = new DepartmentResponse
                {
                    Id = department.Id,
                    Code = department.Code,
                    Name = department.Name,
                    ParentId = department.ParentId,
                    ParentName = department.Parent?.Name,
                    MapId = department.MapId,
                    Level = department.Level,
                    Note = department.Note,
                    IsActive = department.IsActive,
                    CreatedAt = department.CreatedAt,
                    UpdatedAt = department.UpdatedAt,
                    UserCount = department.Users.Count
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = departmentResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department with code: {Code}", code);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new department
        /// </summary>
        /// <param name="request">Department creation request</param>
        /// <returns>Created department</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var department = await _departmentService.CreateDepartmentAsync(
                    request.Code,
                    request.Name,
                    request.ParentId,
                    request.MapId,
                    request.Note);

                _logger.LogInformation("New department created: {Code} with ID: {DepartmentId}", department.Code, department.Id);

                return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, new
                {
                    Status = "Success",
                    Message = "Department created successfully",
                    Data = new
                    {
                        department.Id,
                        department.Code,
                        department.Name,
                        department.ParentId,
                        department.MapId,
                        department.Level,
                        department.Note,
                        department.IsActive,
                        department.CreatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating department: {Code}", request.Code);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating department: {Code}", request.Code);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while creating the department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates an existing department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="request">Department update request</param>
        /// <returns>Updated department</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var department = await _departmentService.UpdateDepartmentAsync(
                    id,
                    request.Code,
                    request.Name,
                    request.ParentId,
                    request.MapId,
                    request.Level,
                    request.Note);
                
                if (department == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Department not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Department updated: {Code} with ID: {DepartmentId}", department.Code, department.Id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Department updated successfully",
                    Data = new
                    {
                        department.Id,
                        department.Code,
                        department.Name,
                        department.ParentId,
                        department.MapId,
                        department.Level,
                        department.Note,
                        department.IsActive,
                        department.CreatedAt,
                        department.UpdatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating department with ID: {DepartmentId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department with ID: {DepartmentId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while updating the department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var success = await _departmentService.DeleteDepartmentAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Department not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Department deleted with ID: {DepartmentId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Department deleted successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while deleting department with ID: {DepartmentId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department with ID: {DepartmentId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while deleting the department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets root departments (departments without parent)
        /// </summary>
        /// <returns>List of root departments</returns>
        [HttpGet("root")]
        public async Task<IActionResult> GetRootDepartments()
        {
            try
            {
                var departments = await _departmentService.GetRootDepartmentsAsync();
                
                var departmentResponses = departments.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    ParentId = d.ParentId,
                    MapId = d.MapId,
                    Level = d.Level,
                    Note = d.Note,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    UserCount = d.Users.Count,
                    Children = d.Children.Select(c => new DepartmentResponse
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        ParentId = c.ParentId,
                        MapId = c.MapId,
                        Level = c.Level,
                        Note = c.Note,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = departmentResponses.Count,
                    Data = departmentResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root departments");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving root departments",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets child departments of a specific parent
        /// </summary>
        /// <param name="parentId">Parent department ID</param>
        /// <returns>List of child departments</returns>
        [HttpGet("parent/{parentId}/children")]
        public async Task<IActionResult> GetChildDepartments(int parentId)
        {
            try
            {
                var departments = await _departmentService.GetChildDepartmentsAsync(parentId);
                
                var departmentResponses = departments.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    ParentId = d.ParentId,
                    MapId = d.MapId,
                    Level = d.Level,
                    Note = d.Note,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    UserCount = d.Users.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = departmentResponses.Count,
                    Data = departmentResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting child departments for parent ID: {ParentId}", parentId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving child departments",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets department hierarchy (department and all its children)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department hierarchy</returns>
        [HttpGet("{id}/hierarchy")]
        public async Task<IActionResult> GetDepartmentHierarchy(int id)
        {
            try
            {
                var departments = await _departmentService.GetDepartmentHierarchyAsync(id);
                
                var departmentResponses = departments.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    ParentId = d.ParentId,
                    ParentName = d.Parent?.Name,
                    MapId = d.MapId,
                    Level = d.Level,
                    Note = d.Note,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    UserCount = d.Users.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = departmentResponses.Count,
                    Data = departmentResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department hierarchy for ID: {DepartmentId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving department hierarchy",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets users in a specific department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>List of users in the department</returns>
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetDepartmentUsers(int id)
        {
            try
            {
                var users = await _departmentService.GetDepartmentUsersAsync(id);
                
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
                    DepartmentId = id,
                    Count = userResponses.Count,
                    Data = userResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users for department ID: {DepartmentId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving department users",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Assigns a user to a department
        /// </summary>
        /// <param name="request">Assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("assign-user")]
        public async Task<IActionResult> AssignUserToDepartment([FromBody] AssignUserToDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _departmentService.AssignUserToDepartmentAsync(request.UserId, request.DepartmentId);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to assign user to department. User or department not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("User {UserId} assigned to department {DepartmentId}", request.UserId, request.DepartmentId);

                return Ok(new
                {
                    Status = "Success",
                    Message = "User assigned to department successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning user {UserId} to department {DepartmentId}", request.UserId, request.DepartmentId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while assigning user to department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Removes a user from their department
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Removal result</returns>
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> RemoveUserFromDepartment(int userId)
        {
            try
            {
                var success = await _departmentService.RemoveUserFromDepartmentAsync(userId);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "User not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("User {UserId} removed from department", userId);

                return Ok(new
                {
                    Status = "Success",
                    Message = "User removed from department successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing user {UserId} from department", userId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while removing user from department",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Searches departments by term
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching departments</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchDepartments([FromQuery] string searchTerm)
        {
            try
            {
                var departments = await _departmentService.SearchDepartmentsAsync(searchTerm);
                
                var departmentResponses = departments.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    ParentId = d.ParentId,
                    ParentName = d.Parent?.Name,
                    MapId = d.MapId,
                    Level = d.Level,
                    Note = d.Note,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    UserCount = d.Users.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    SearchTerm = searchTerm,
                    Count = departmentResponses.Count,
                    Data = departmentResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching departments with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while searching departments",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets departments by level
        /// </summary>
        /// <param name="level">Department level</param>
        /// <returns>Departments at the specified level</returns>
        [HttpGet("by-level/{level}")]
        public async Task<IActionResult> GetDepartmentsByLevel(int level)
        {
            try
            {
                var departments = await _departmentService.GetDepartmentsByLevelAsync(level);
                
                var departmentResponses = departments.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    ParentId = d.ParentId,
                    ParentName = d.Parent?.Name,
                    MapId = d.MapId,
                    Level = d.Level,
                    Note = d.Note,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    UserCount = d.Users.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Level = level,
                    Count = departmentResponses.Count,
                    Data = departmentResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting departments by level: {Level}", level);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving departments by level",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
