using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    /// <summary>
    /// Service interface for Menu management
    /// </summary>
    public interface IMenuService
    {
        // Basic CRUD operations
        Task<IEnumerable<Menu>> GetAllMenusAsync();
        Task<Menu?> GetMenuByIdAsync(int id);
        Task<Menu> CreateMenuAsync(string name, string? href = null, string? icon = null, int order = 0, int? parentId = null, string? description = null, bool isVisible = true, string? target = "_self", string? cssClass = null, string? dataAttributes = null);
        Task<Menu?> UpdateMenuAsync(int id, string name, string? href = null, string? icon = null, int order = 0, int? parentId = null, int level = 0, string? description = null, bool isVisible = true, string? target = "_self", string? cssClass = null, string? dataAttributes = null);
        Task<bool> DeleteMenuAsync(int id);

        // Hierarchical operations
        Task<IEnumerable<Menu>> GetRootMenusAsync();
        Task<IEnumerable<Menu>> GetChildMenusAsync(int parentId);
        Task<IEnumerable<Menu>> GetMenuHierarchyAsync(int menuId);
        Task<int> CalculateMenuLevelAsync(int menuId);

        // Ordering operations
        Task<bool> ReorderMenusAsync(int parentId, List<int> menuIds);
        Task<bool> MoveMenuUpAsync(int menuId);
        Task<bool> MoveMenuDownAsync(int menuId);

        // Visibility operations
        Task<IEnumerable<Menu>> GetVisibleMenusAsync();
        Task<bool> ToggleMenuVisibilityAsync(int menuId);

        // Search and filter operations
        Task<IEnumerable<Menu>> SearchMenusAsync(string searchTerm);
        Task<IEnumerable<Menu>> GetMenusByLevelAsync(int level);
        Task<IEnumerable<Menu>> GetMenusByParentAsync(int? parentId);

        // Navigation operations
        Task<IEnumerable<Menu>> GetNavigationMenuAsync();
        Task<IEnumerable<Menu>> GetBreadcrumbAsync(int menuId);
    }
}
