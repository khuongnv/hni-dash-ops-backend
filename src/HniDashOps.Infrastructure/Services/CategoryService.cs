using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Category management
    /// </summary>
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.Level)
                    .ThenBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories");
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<Category?> GetCategoryByCodeAsync(string code)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with code: {Code}", code);
                throw;
            }
        }

        public async Task<Category> CreateCategoryAsync(string code, string name, int? parentId = null, string? mapId = null, string? note = null, int order = 0, string? color = null, string? icon = null, bool isVisible = true, string? type = null, string? metadata = null)
        {
            try
            {
                // Validate code uniqueness
                if (!await IsCodeUniqueAsync(code))
                {
                    throw new InvalidOperationException($"Category with code '{code}' already exists");
                }

                // Validate MapId uniqueness if provided
                if (!string.IsNullOrEmpty(mapId) && !await IsMapIdUniqueAsync(mapId))
                {
                    throw new InvalidOperationException($"Category with MapId '{mapId}' already exists");
                }

                // Calculate level
                int level = 0;
                if (parentId.HasValue)
                {
                    var parent = await _context.Categories.FindAsync(parentId.Value);
                    if (parent == null || parent.IsDeleted)
                    {
                        throw new InvalidOperationException($"Parent category with ID {parentId} not found");
                    }
                    level = parent.Level + 1;
                }

                var category = new Category
                {
                    Code = code,
                    Name = name,
                    ParentId = parentId,
                    MapId = mapId,
                    Level = level,
                    Note = note,
                    Order = order,
                    Color = color,
                    Icon = icon,
                    IsVisible = isVisible,
                    Type = type,
                    Metadata = metadata,
                    IsActive = true
                };

                // Set audit fields for creation
                SetCreatedAuditFields(category);

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Category created: {Name} with code: {Code}", name, code);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category: {Name}", name);
                throw;
            }
        }

        public async Task<Category?> UpdateCategoryAsync(int id, string code, string name, int? parentId = null, string? mapId = null, int level = 0, string? note = null, int order = 0, string? color = null, string? icon = null, bool isVisible = true, string? type = null, string? metadata = null)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null || category.IsDeleted)
                {
                    return null;
                }

                // Validate code uniqueness (excluding current category)
                if (!await IsCodeUniqueAsync(code, id))
                {
                    throw new InvalidOperationException($"Category with code '{code}' already exists");
                }

                // Validate MapId uniqueness if provided (excluding current category)
                if (!string.IsNullOrEmpty(mapId) && !await IsMapIdUniqueAsync(mapId, id))
                {
                    throw new InvalidOperationException($"Category with MapId '{mapId}' already exists");
                }

                // Validate parent relationship
                if (parentId.HasValue)
                {
                    if (parentId.Value == id)
                    {
                        throw new InvalidOperationException("Category cannot be its own parent");
                    }

                    var parent = await _context.Categories.FindAsync(parentId.Value);
                    if (parent == null || parent.IsDeleted)
                    {
                        throw new InvalidOperationException($"Parent category with ID {parentId} not found");
                    }

                    // Check for circular reference
                    if (await IsCircularReferenceAsync(id, parentId.Value))
                    {
                        throw new InvalidOperationException("Circular reference detected in category hierarchy");
                    }

                    level = parent.Level + 1;
                }

                category.Code = code;
                category.Name = name;
                category.ParentId = parentId;
                category.MapId = mapId;
                category.Level = level;
                category.Note = note;
                category.Order = order;
                category.Color = color;
                category.Icon = icon;
                category.IsVisible = isVisible;
                category.Type = type;
                category.Metadata = metadata;

                // Set audit fields for update
                SetUpdatedAuditFields(category);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Category updated: {Name} with code: {Code}", name, code);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null || category.IsDeleted)
                {
                    return false;
                }

                // Check if category has children
                var hasChildren = await _context.Categories.AnyAsync(c => c.ParentId == id && !c.IsDeleted);
                if (hasChildren)
                {
                    throw new InvalidOperationException("Cannot delete category that has child categories");
                }

                // Soft delete
                SetDeletedAuditFields(category);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Category deleted with ID: {CategoryId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Children)
                    .Where(c => c.ParentId == null && !c.IsDeleted)
                    .OrderBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root categories");
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Children)
                    .Where(c => c.ParentId == parentId && !c.IsDeleted)
                    .OrderBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting child categories for parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync(int categoryId)
        {
            try
            {
                var categories = new List<Category>();
                var category = await GetCategoryByIdAsync(categoryId);
                
                if (category != null)
                {
                    categories.Add(category);
                    
                    // Get all children recursively
                    var children = await GetChildCategoriesAsync(categoryId);
                    foreach (var child in children)
                    {
                        var childHierarchy = await GetCategoryHierarchyAsync(child.Id);
                        categories.AddRange(childHierarchy);
                    }
                }

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category hierarchy for ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<int> CalculateCategoryLevelAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null || category.IsDeleted)
                {
                    return -1;
                }

                int level = 0;
                var current = category;
                
                while (current.ParentId.HasValue)
                {
                    level++;
                    current = await _context.Categories.FindAsync(current.ParentId.Value);
                    if (current == null || current.IsDeleted)
                    {
                        break;
                    }
                }

                return level;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating category level for ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<bool> ReorderCategoriesAsync(int parentId, List<int> categoryIds)
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.ParentId == parentId && !c.IsDeleted)
                    .ToListAsync();

                for (int i = 0; i < categoryIds.Count; i++)
                {
                    var category = categories.FirstOrDefault(c => c.Id == categoryIds[i]);
                    if (category != null)
                    {
                        category.Order = i + 1;
                        category.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reordering categories for parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<bool> MoveCategoryUpAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null || category.IsDeleted)
                {
                    return false;
                }

                var siblingCategories = await _context.Categories
                    .Where(c => c.ParentId == category.ParentId && c.Id != categoryId && !c.IsDeleted)
                    .OrderBy(c => c.Order)
                    .ToListAsync();

                var currentIndex = siblingCategories.FindIndex(c => c.Order > category.Order);
                if (currentIndex > 0)
                {
                    var previousCategory = siblingCategories[currentIndex - 1];
                    var tempOrder = category.Order;
                    category.Order = previousCategory.Order;
                    previousCategory.Order = tempOrder;

                    category.UpdatedAt = DateTime.UtcNow;
                    previousCategory.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving category up with ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<bool> MoveCategoryDownAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null || category.IsDeleted)
                {
                    return false;
                }

                var siblingCategories = await _context.Categories
                    .Where(c => c.ParentId == category.ParentId && c.Id != categoryId && !c.IsDeleted)
                    .OrderBy(c => c.Order)
                    .ToListAsync();

                var currentIndex = siblingCategories.FindIndex(c => c.Order > category.Order);
                if (currentIndex < siblingCategories.Count - 1)
                {
                    var nextCategory = siblingCategories[currentIndex + 1];
                    var tempOrder = category.Order;
                    category.Order = nextCategory.Order;
                    nextCategory.Order = tempOrder;

                    category.UpdatedAt = DateTime.UtcNow;
                    nextCategory.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving category down with ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetVisibleCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children.Where(ch => ch.IsVisible && !ch.IsDeleted))
                    .Where(c => c.IsVisible && !c.IsDeleted)
                    .OrderBy(c => c.Level)
                    .ThenBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting visible categories");
                throw;
            }
        }

        public async Task<bool> ToggleCategoryVisibilityAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null || category.IsDeleted)
                {
                    return false;
                }

                category.IsVisible = !category.IsVisible;
                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Category visibility toggled for ID: {CategoryId}, now visible: {IsVisible}", categoryId, category.IsVisible);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling category visibility for ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllCategoriesAsync();
                }

                var term = searchTerm.ToLower();
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Where(c => !c.IsDeleted && 
                               (c.Name.ToLower().Contains(term) || 
                                c.Code.ToLower().Contains(term) ||
                                (c.Note != null && c.Note.ToLower().Contains(term)) ||
                                (c.MapId != null && c.MapId.ToLower().Contains(term))))
                    .OrderBy(c => c.Level)
                    .ThenBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching categories with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesByLevelAsync(int level)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Where(c => c.Level == level && !c.IsDeleted)
                    .OrderBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories by level: {Level}", level);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesByParentAsync(int? parentId)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Children)
                    .Where(c => c.ParentId == parentId && !c.IsDeleted)
                    .OrderBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories by parent ID: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string type)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Where(c => c.Type == type && !c.IsDeleted)
                    .OrderBy(c => c.Level)
                    .ThenBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories by type: {Type}", type);
                throw;
            }
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            try
            {
                var query = _context.Categories.Where(c => c.Code == code && !c.IsDeleted);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeId.Value);
                }

                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking code uniqueness: {Code}", code);
                throw;
            }
        }

        public async Task<bool> IsMapIdUniqueAsync(string mapId, int? excludeId = null)
        {
            try
            {
                var query = _context.Categories.Where(c => c.MapId == mapId && !c.IsDeleted);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeId.Value);
                }

                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking MapId uniqueness: {MapId}", mapId);
                throw;
            }
        }

        public async Task<Category?> GetCategoryByMapIdAsync(string mapId)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.MapId == mapId && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with MapId: {MapId}", mapId);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoryTreeAsync()
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Children.Where(ch => ch.IsVisible && !ch.IsDeleted))
                    .Where(c => c.IsVisible && !c.IsDeleted && c.ParentId == null)
                    .OrderBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category tree");
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId)
        {
            try
            {
                var path = new List<Category>();
                var category = await _context.Categories.FindAsync(categoryId);
                
                while (category != null && !category.IsDeleted)
                {
                    path.Insert(0, category);
                    category = category.ParentId.HasValue ? await _context.Categories.FindAsync(category.ParentId.Value) : null;
                }

                return path;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category path for ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<bool> HasChildrenAsync(int categoryId)
        {
            try
            {
                return await _context.Categories.AnyAsync(c => c.ParentId == categoryId && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if category has children: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<int> GetChildrenCountAsync(int categoryId)
        {
            try
            {
                return await _context.Categories.CountAsync(c => c.ParentId == categoryId && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting children count for category: {CategoryId}", categoryId);
                throw;
            }
        }

        private async Task<bool> IsCircularReferenceAsync(int categoryId, int parentId)
        {
            try
            {
                var current = await _context.Categories.FindAsync(parentId);
                while (current != null && !current.IsDeleted)
                {
                    if (current.Id == categoryId)
                    {
                        return true;
                    }
                    current = current.ParentId.HasValue ? await _context.Categories.FindAsync(current.ParentId.Value) : null;
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
