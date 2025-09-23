using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Menu management
    /// </summary>
    public class MenuService : BaseService, IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuService> _logger;

        public MenuService(ApplicationDbContext context, ILogger<MenuService> logger, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Menu>> GetAllMenusAsync()
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Children)
                    .Where(m => !m.IsDeleted)
                    .OrderBy(m => m.Level)
                    .ThenBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all menus");
                throw;
            }
        }

        public async Task<Menu?> GetMenuByIdAsync(int id)
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Children)
                    .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu with ID: {MenuId}", id);
                throw;
            }
        }

        public async Task<Menu> CreateMenuAsync(string name, string? href = null, string? icon = null, int order = 0, int? parentId = null, string? description = null, bool isVisible = true, string? target = "_self", string? cssClass = null, string? dataAttributes = null)
        {
            try
            {
                // Calculate level
                int level = 0;
                if (parentId.HasValue)
                {
                    var parent = await _context.Menus.FindAsync(parentId.Value);
                    if (parent == null || parent.IsDeleted)
                    {
                        throw new InvalidOperationException($"Parent menu with ID {parentId} not found");
                    }
                    level = parent.Level + 1;
                }

                var menu = new Menu
                {
                    Name = name,
                    Href = href,
                    Icon = icon,
                    Order = order,
                    ParentId = parentId,
                    Level = level,
                    Description = description,
                    IsVisible = isVisible,
                    Target = target ?? "_self",
                    CssClass = cssClass,
                    DataAttributes = dataAttributes,
                    IsActive = true
                };

                // Set audit fields for creation
                SetCreatedAuditFields(menu);

                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Menu created: {Name} with ID: {MenuId}", name, menu.Id);
                return menu;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating menu: {Name}", name);
                throw;
            }
        }

        public async Task<Menu?> UpdateMenuAsync(int id, string name, string? href = null, string? icon = null, int order = 0, int? parentId = null, int level = 0, string? description = null, bool isVisible = true, string? target = "_self", string? cssClass = null, string? dataAttributes = null)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(id);
                if (menu == null || menu.IsDeleted)
                {
                    return null;
                }

                // Validate parent relationship
                if (parentId.HasValue)
                {
                    if (parentId.Value == id)
                    {
                        throw new InvalidOperationException("Menu cannot be its own parent");
                    }

                    var parent = await _context.Menus.FindAsync(parentId.Value);
                    if (parent == null || parent.IsDeleted)
                    {
                        throw new InvalidOperationException($"Parent menu with ID {parentId} not found");
                    }

                    // Check for circular reference
                    if (await IsCircularReferenceAsync(id, parentId.Value))
                    {
                        throw new InvalidOperationException("Circular reference detected in menu hierarchy");
                    }

                    level = parent.Level + 1;
                }

                menu.Name = name;
                menu.Href = href;
                menu.Icon = icon;
                menu.Order = order;
                menu.ParentId = parentId;
                menu.Level = level;
                menu.Description = description;
                menu.IsVisible = isVisible;
                menu.Target = target ?? "_self";
                menu.CssClass = cssClass;
                menu.DataAttributes = dataAttributes;

                // Set audit fields for update
                SetUpdatedAuditFields(menu);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Menu updated: {Name} with ID: {MenuId}", name, menu.Id);
                return menu;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating menu with ID: {MenuId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(id);
                if (menu == null || menu.IsDeleted)
                {
                    return false;
                }

                // Check if menu has children
                var hasChildren = await _context.Menus.AnyAsync(m => m.ParentId == id && !m.IsDeleted);
                if (hasChildren)
                {
                    throw new InvalidOperationException("Cannot delete menu that has child menus");
                }

                // Soft delete
                SetDeletedAuditFields(menu);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Menu deleted with ID: {MenuId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting menu with ID: {MenuId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetRootMenusAsync()
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Children)
                    .Where(m => m.ParentId == null && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root menus");
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetChildMenusAsync(int parentId)
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Children)
                    .Where(m => m.ParentId == parentId && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting child menus for parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetMenuHierarchyAsync(int menuId)
        {
            try
            {
                var menus = new List<Menu>();
                var menu = await GetMenuByIdAsync(menuId);
                
                if (menu != null)
                {
                    menus.Add(menu);
                    
                    // Get all children recursively
                    var children = await GetChildMenusAsync(menuId);
                    foreach (var child in children)
                    {
                        var childHierarchy = await GetMenuHierarchyAsync(child.Id);
                        menus.AddRange(childHierarchy);
                    }
                }

                return menus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu hierarchy for ID: {MenuId}", menuId);
                throw;
            }
        }

        public async Task<int> CalculateMenuLevelAsync(int menuId)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(menuId);
                if (menu == null || menu.IsDeleted)
                {
                    return -1;
                }

                int level = 0;
                var current = menu;
                
                while (current.ParentId.HasValue)
                {
                    level++;
                    current = await _context.Menus.FindAsync(current.ParentId.Value);
                    if (current == null || current.IsDeleted)
                    {
                        break;
                    }
                }

                return level;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating menu level for ID: {MenuId}", menuId);
                throw;
            }
        }

        public async Task<bool> ReorderMenusAsync(int parentId, List<int> menuIds)
        {
            try
            {
                var menus = await _context.Menus
                    .Where(m => m.ParentId == parentId && !m.IsDeleted)
                    .ToListAsync();

                for (int i = 0; i < menuIds.Count; i++)
                {
                    var menu = menus.FirstOrDefault(m => m.Id == menuIds[i]);
                    if (menu != null)
                    {
                        menu.Order = i + 1;
                        menu.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reordering menus for parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<bool> MoveMenuUpAsync(int menuId)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(menuId);
                if (menu == null || menu.IsDeleted)
                {
                    return false;
                }

                var siblingMenus = await _context.Menus
                    .Where(m => m.ParentId == menu.ParentId && m.Id != menuId && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

                var currentIndex = siblingMenus.FindIndex(m => m.Order > menu.Order);
                if (currentIndex > 0)
                {
                    var previousMenu = siblingMenus[currentIndex - 1];
                    var tempOrder = menu.Order;
                    menu.Order = previousMenu.Order;
                    previousMenu.Order = tempOrder;

                    menu.UpdatedAt = DateTime.UtcNow;
                    previousMenu.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving menu up with ID: {MenuId}", menuId);
                throw;
            }
        }

        public async Task<bool> MoveMenuDownAsync(int menuId)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(menuId);
                if (menu == null || menu.IsDeleted)
                {
                    return false;
                }

                var siblingMenus = await _context.Menus
                    .Where(m => m.ParentId == menu.ParentId && m.Id != menuId && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

                var currentIndex = siblingMenus.FindIndex(m => m.Order > menu.Order);
                if (currentIndex < siblingMenus.Count - 1)
                {
                    var nextMenu = siblingMenus[currentIndex + 1];
                    var tempOrder = menu.Order;
                    menu.Order = nextMenu.Order;
                    nextMenu.Order = tempOrder;

                    menu.UpdatedAt = DateTime.UtcNow;
                    nextMenu.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving menu down with ID: {MenuId}", menuId);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetVisibleMenusAsync()
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Children.Where(c => c.IsVisible && !c.IsDeleted))
                    .Where(m => m.IsVisible && !m.IsDeleted)
                    .OrderBy(m => m.Level)
                    .ThenBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting visible menus");
                throw;
            }
        }

        public async Task<bool> ToggleMenuVisibilityAsync(int menuId)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(menuId);
                if (menu == null || menu.IsDeleted)
                {
                    return false;
                }

                menu.IsVisible = !menu.IsVisible;
                menu.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Menu visibility toggled for ID: {MenuId}, now visible: {IsVisible}", menuId, menu.IsVisible);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling menu visibility for ID: {MenuId}", menuId);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> SearchMenusAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllMenusAsync();
                }

                var term = searchTerm.ToLower();
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Where(m => !m.IsDeleted && 
                               (m.Name.ToLower().Contains(term) || 
                                (m.Description != null && m.Description.ToLower().Contains(term)) ||
                                (m.Href != null && m.Href.ToLower().Contains(term))))
                    .OrderBy(m => m.Level)
                    .ThenBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching menus with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetMenusByLevelAsync(int level)
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Children)
                    .Where(m => m.Level == level && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menus by level: {Level}", level);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetMenusByParentAsync(int? parentId)
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Children)
                    .Where(m => m.ParentId == parentId && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menus by parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetNavigationMenuAsync()
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Children.Where(c => c.IsVisible && !c.IsDeleted))
                    .Where(m => m.IsVisible && !m.IsDeleted && m.ParentId == null)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting navigation menu");
                throw;
            }
        }

        public async Task<IEnumerable<Menu>> GetBreadcrumbAsync(int menuId)
        {
            try
            {
                var breadcrumb = new List<Menu>();
                var menu = await _context.Menus.FindAsync(menuId);
                
                while (menu != null && !menu.IsDeleted)
                {
                    breadcrumb.Insert(0, menu);
                    menu = menu.ParentId.HasValue ? await _context.Menus.FindAsync(menu.ParentId.Value) : null;
                }

                return breadcrumb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting breadcrumb for menu ID: {MenuId}", menuId);
                throw;
            }
        }

        private async Task<bool> IsCircularReferenceAsync(int menuId, int parentId)
        {
            try
            {
                var current = await _context.Menus.FindAsync(parentId);
                while (current != null && !current.IsDeleted)
                {
                    if (current.Id == menuId)
                    {
                        return true;
                    }
                    current = current.ParentId.HasValue ? await _context.Menus.FindAsync(current.ParentId.Value) : null;
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
