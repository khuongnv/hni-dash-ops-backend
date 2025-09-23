using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Department management
    /// </summary>
    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(ApplicationDbContext context, ILogger<DepartmentService> logger, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Parent)
                    .Include(d => d.Children)
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.Level)
                    .ThenBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all departments");
                throw;
            }
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Parent)
                    .Include(d => d.Children)
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department with ID: {DepartmentId}", id);
                throw;
            }
        }

        public async Task<Department?> GetDepartmentByCodeAsync(string code)
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Parent)
                    .Include(d => d.Children)
                    .FirstOrDefaultAsync(d => d.Code == code && !d.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department with code: {Code}", code);
                throw;
            }
        }

        public async Task<Department> CreateDepartmentAsync(string code, string name, int? parentId = null, string? mapId = null, string? note = null)
        {
            try
            {
                // Check if code already exists
                if (await IsDepartmentCodeExistsAsync(code))
                {
                    throw new InvalidOperationException($"Department with code '{code}' already exists");
                }

                // Check if mapId already exists (if provided)
                if (!string.IsNullOrEmpty(mapId) && await IsMapIdExistsAsync(mapId))
                {
                    throw new InvalidOperationException($"Department with MapId '{mapId}' already exists");
                }

                // Calculate level
                int level = 0;
                if (parentId.HasValue)
                {
                    var parent = await _context.Departments.FindAsync(parentId.Value);
                    if (parent == null || parent.IsDeleted)
                    {
                        throw new InvalidOperationException($"Parent department with ID {parentId} not found");
                    }
                    level = parent.Level + 1;
                }

                var department = new Department
                {
                    Code = code,
                    Name = name,
                    ParentId = parentId,
                    MapId = mapId,
                    Level = level,
                    Note = note,
                    IsActive = true
                };

                // Set audit fields for creation
                SetCreatedAuditFields(department);

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Department created: {Code} with ID: {DepartmentId}", code, department.Id);
                return department;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating department: {Code}", code);
                throw;
            }
        }

        public async Task<Department?> UpdateDepartmentAsync(int id, string code, string name, int? parentId = null, string? mapId = null, int level = 0, string? note = null)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null || department.IsDeleted)
                {
                    return null;
                }

                // Check if code already exists (excluding current department)
                if (await IsDepartmentCodeExistsAsync(code, id))
                {
                    throw new InvalidOperationException($"Department with code '{code}' already exists");
                }

                // Check if mapId already exists (excluding current department)
                if (!string.IsNullOrEmpty(mapId) && await IsMapIdExistsAsync(mapId, id))
                {
                    throw new InvalidOperationException($"Department with MapId '{mapId}' already exists");
                }

                // Validate parent relationship
                if (parentId.HasValue)
                {
                    if (parentId.Value == id)
                    {
                        throw new InvalidOperationException("Department cannot be its own parent");
                    }

                    var parent = await _context.Departments.FindAsync(parentId.Value);
                    if (parent == null || parent.IsDeleted)
                    {
                        throw new InvalidOperationException($"Parent department with ID {parentId} not found");
                    }

                    // Check for circular reference
                    if (await IsCircularReferenceAsync(id, parentId.Value))
                    {
                        throw new InvalidOperationException("Circular reference detected in department hierarchy");
                    }

                    level = parent.Level + 1;
                }

                department.Code = code;
                department.Name = name;
                department.ParentId = parentId;
                department.MapId = mapId;
                department.Level = level;
                department.Note = note;

                // Set audit fields for update
                SetUpdatedAuditFields(department);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Department updated: {Code} with ID: {DepartmentId}", code, department.Id);
                return department;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department with ID: {DepartmentId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null || department.IsDeleted)
                {
                    return false;
                }

                // Check if department has children
                var hasChildren = await _context.Departments.AnyAsync(d => d.ParentId == id && !d.IsDeleted);
                if (hasChildren)
                {
                    throw new InvalidOperationException("Cannot delete department that has child departments");
                }

                // Check if department has users
                var hasUsers = await _context.Users.AnyAsync(u => u.DepartmentId == id && !u.IsDeleted);
                if (hasUsers)
                {
                    throw new InvalidOperationException("Cannot delete department that has assigned users");
                }

                // Soft delete
                SetDeletedAuditFields(department);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Department deleted with ID: {DepartmentId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department with ID: {DepartmentId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Department>> GetRootDepartmentsAsync()
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Children)
                    .Where(d => d.ParentId == null && !d.IsDeleted)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root departments");
                throw;
            }
        }

        public async Task<IEnumerable<Department>> GetChildDepartmentsAsync(int parentId)
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Children)
                    .Where(d => d.ParentId == parentId && !d.IsDeleted)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting child departments for parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<IEnumerable<Department>> GetDepartmentHierarchyAsync(int departmentId)
        {
            try
            {
                var departments = new List<Department>();
                var department = await GetDepartmentByIdAsync(departmentId);
                
                if (department != null)
                {
                    departments.Add(department);
                    
                    // Get all children recursively
                    var children = await GetChildDepartmentsAsync(departmentId);
                    foreach (var child in children)
                    {
                        var childHierarchy = await GetDepartmentHierarchyAsync(child.Id);
                        departments.AddRange(childHierarchy);
                    }
                }

                return departments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department hierarchy for ID: {DepartmentId}", departmentId);
                throw;
            }
        }

        public async Task<bool> IsDepartmentCodeExistsAsync(string code, int? excludeId = null)
        {
            try
            {
                var query = _context.Departments.Where(d => d.Code == code && !d.IsDeleted);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(d => d.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if department code exists: {Code}", code);
                throw;
            }
        }

        public async Task<bool> IsMapIdExistsAsync(string mapId, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(mapId))
                    return false;

                var query = _context.Departments.Where(d => d.MapId == mapId && !d.IsDeleted);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(d => d.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if MapId exists: {MapId}", mapId);
                throw;
            }
        }

        public async Task<int> CalculateDepartmentLevelAsync(int departmentId)
        {
            try
            {
                var department = await _context.Departments.FindAsync(departmentId);
                if (department == null || department.IsDeleted)
                {
                    return -1;
                }

                int level = 0;
                var current = department;
                
                while (current.ParentId.HasValue)
                {
                    level++;
                    current = await _context.Departments.FindAsync(current.ParentId.Value);
                    if (current == null || current.IsDeleted)
                    {
                        break;
                    }
                }

                return level;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating department level for ID: {DepartmentId}", departmentId);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetDepartmentUsersAsync(int departmentId)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Department)
                    .Include(u => u.GroupUsers)
                        .ThenInclude(gu => gu.Group)
                    .Where(u => u.DepartmentId == departmentId && !u.IsDeleted)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users for department ID: {DepartmentId}", departmentId);
                throw;
            }
        }

        public async Task<bool> AssignUserToDepartmentAsync(int userId, int departmentId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return false;
                }

                var department = await _context.Departments.FindAsync(departmentId);
                if (department == null || department.IsDeleted)
                {
                    return false;
                }

                user.DepartmentId = departmentId;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} assigned to department {DepartmentId}", userId, departmentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning user {UserId} to department {DepartmentId}", userId, departmentId);
                throw;
            }
        }

        public async Task<bool> RemoveUserFromDepartmentAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return false;
                }

                user.DepartmentId = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} removed from department", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing user {UserId} from department", userId);
                throw;
            }
        }

        public async Task<IEnumerable<Department>> SearchDepartmentsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllDepartmentsAsync();
                }

                var term = searchTerm.ToLower();
                return await _context.Departments
                    .Include(d => d.Parent)
                    .Where(d => !d.IsDeleted && 
                               (d.Code.ToLower().Contains(term) || 
                                d.Name.ToLower().Contains(term) ||
                                (d.Note != null && d.Note.ToLower().Contains(term))))
                    .OrderBy(d => d.Level)
                    .ThenBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching departments with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Department>> GetDepartmentsByLevelAsync(int level)
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Parent)
                    .Include(d => d.Children)
                    .Where(d => d.Level == level && !d.IsDeleted)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting departments by level: {Level}", level);
                throw;
            }
        }

        private async Task<bool> IsCircularReferenceAsync(int departmentId, int parentId)
        {
            try
            {
                var current = await _context.Departments.FindAsync(parentId);
                while (current != null && !current.IsDeleted)
                {
                    if (current.Id == departmentId)
                    {
                        return true;
                    }
                    current = current.ParentId.HasValue ? await _context.Departments.FindAsync(current.ParentId.Value) : null;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking circular reference");
                throw;
            }
        }
    }
}
