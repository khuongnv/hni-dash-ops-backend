using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Service interface for Category management
    /// </summary>
    public interface ICategoryService
    {
        // Basic CRUD operations
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryByCodeAsync(string code);
        Task<Category> CreateCategoryAsync(string code, string name, int? parentId = null, string? mapId = null, string? note = null, int order = 0, string? color = null, string? icon = null, bool isVisible = true, string? type = null, string? metadata = null);
        Task<Category?> UpdateCategoryAsync(int id, string code, string name, int? parentId = null, string? mapId = null, int level = 0, string? note = null, int order = 0, string? color = null, string? icon = null, bool isVisible = true, string? type = null, string? metadata = null);
        Task<bool> DeleteCategoryAsync(int id);

        // Hierarchical operations
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
        Task<IEnumerable<Category>> GetCategoryHierarchyAsync(int categoryId);
        Task<int> CalculateCategoryLevelAsync(int categoryId);

        // Ordering operations
        Task<bool> ReorderCategoriesAsync(int parentId, List<int> categoryIds);
        Task<bool> MoveCategoryUpAsync(int categoryId);
        Task<bool> MoveCategoryDownAsync(int categoryId);

        // Visibility operations
        Task<IEnumerable<Category>> GetVisibleCategoriesAsync();
        Task<bool> ToggleCategoryVisibilityAsync(int categoryId);

        // Search and filter operations
        Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
        Task<IEnumerable<Category>> GetCategoriesByLevelAsync(int level);
        Task<IEnumerable<Category>> GetCategoriesByParentAsync(int? parentId);
        Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string type);

        // Code and MapId operations
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> IsMapIdUniqueAsync(string mapId, int? excludeId = null);
        Task<Category?> GetCategoryByMapIdAsync(string mapId);

        // Tree operations
        Task<IEnumerable<Category>> GetCategoryTreeAsync();
        Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId);
        Task<bool> HasChildrenAsync(int categoryId);
        Task<int> GetChildrenCountAsync(int categoryId);
    }
}
