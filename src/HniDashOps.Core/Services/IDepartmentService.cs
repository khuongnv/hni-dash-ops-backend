using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Service interface for Department management
    /// </summary>
    public interface IDepartmentService
    {
        // Basic CRUD operations
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<Department?> GetDepartmentByCodeAsync(string code);
        Task<Department> CreateDepartmentAsync(string code, string name, int? parentId = null, string? mapId = null, string? note = null);
        Task<Department?> UpdateDepartmentAsync(int id, string code, string name, int? parentId = null, string? mapId = null, int level = 0, string? note = null);
        Task<bool> DeleteDepartmentAsync(int id);

        // Hierarchical operations
        Task<IEnumerable<Department>> GetRootDepartmentsAsync();
        Task<IEnumerable<Department>> GetChildDepartmentsAsync(int parentId);
        Task<IEnumerable<Department>> GetDepartmentHierarchyAsync(int departmentId);
        Task<bool> IsDepartmentCodeExistsAsync(string code, int? excludeId = null);
        Task<bool> IsMapIdExistsAsync(string mapId, int? excludeId = null);
        Task<int> CalculateDepartmentLevelAsync(int departmentId);

        // User-Department operations
        Task<IEnumerable<User>> GetDepartmentUsersAsync(int departmentId);
        Task<bool> AssignUserToDepartmentAsync(int userId, int departmentId);
        Task<bool> RemoveUserFromDepartmentAsync(int userId);

        // Search and filter operations
        Task<IEnumerable<Department>> SearchDepartmentsAsync(string searchTerm);
        Task<IEnumerable<Department>> GetDepartmentsByLevelAsync(int level);
    }
}
