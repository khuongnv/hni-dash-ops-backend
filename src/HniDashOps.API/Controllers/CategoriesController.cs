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
    /// Controller for managing categories
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeSuperAdmin]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    MapId = c.MapId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving categories",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a specific category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                
                if (category == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Category not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Code = category.Code,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    MapId = category.MapId,
                    Level = category.Level,
                    Note = category.Note,
                    Order = category.Order,
                    Color = category.Color,
                    Icon = category.Icon,
                    IsVisible = category.IsVisible,
                    Type = category.Type,
                    Metadata = category.Metadata,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt,
                    Children = category.Children.Select(ch => new CategoryResponse
                    {
                        Id = ch.Id,
                        Code = ch.Code,
                        Name = ch.Name,
                        ParentId = ch.ParentId,
                        Level = ch.Level,
                        Note = ch.Note,
                        Order = ch.Order,
                        Color = ch.Color,
                        Icon = ch.Icon,
                        IsVisible = ch.IsVisible,
                        Type = ch.Type,
                        Metadata = ch.Metadata,
                        IsActive = ch.IsActive,
                        CreatedAt = ch.CreatedAt,
                        UpdatedAt = ch.UpdatedAt,
                        ChildrenCount = ch.Children.Count
                    }).ToList(),
                    ChildrenCount = category.Children.Count
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = categoryResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the category",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a category by code
        /// </summary>
        /// <param name="code">Category code</param>
        /// <returns>Category details</returns>
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetCategoryByCode(string code)
        {
            try
            {
                var category = await _categoryService.GetCategoryByCodeAsync(code);
                
                if (category == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Category not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Code = category.Code,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    MapId = category.MapId,
                    Level = category.Level,
                    Note = category.Note,
                    Order = category.Order,
                    Color = category.Color,
                    Icon = category.Icon,
                    IsVisible = category.IsVisible,
                    Type = category.Type,
                    Metadata = category.Metadata,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt,
                    ChildrenCount = category.Children.Count
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = categoryResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with code: {Code}", code);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the category",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="request">Category creation request</param>
        /// <returns>Created category</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _categoryService.CreateCategoryAsync(
                    request.Code,
                    request.Name,
                    request.ParentId,
                    request.MapId,
                    request.Note,
                    request.Order,
                    request.Color,
                    request.Icon,
                    request.IsVisible,
                    request.Type,
                    request.Metadata);

                _logger.LogInformation("New category created: {Name} with code: {Code}", category.Name, category.Code);

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new
                {
                    Status = "Success",
                    Message = "Category created successfully",
                    Data = new
                    {
                        category.Id,
                        category.Code,
                        category.Name,
                        category.ParentId,
                        category.MapId,
                        category.Level,
                        category.Note,
                        category.Order,
                        category.Color,
                        category.Icon,
                        category.IsVisible,
                        category.Type,
                        category.Metadata,
                        category.IsActive,
                        category.CreatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating category: {Name}", request.Name);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category: {Name}", request.Name);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while creating the category",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="request">Category update request</param>
        /// <returns>Updated category</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _categoryService.UpdateCategoryAsync(
                    id,
                    request.Code,
                    request.Name,
                    request.ParentId,
                    request.MapId,
                    request.Level,
                    request.Note,
                    request.Order,
                    request.Color,
                    request.Icon,
                    request.IsVisible,
                    request.Type,
                    request.Metadata);
                
                if (category == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Category not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Category updated: {Name} with code: {Code}", category.Name, category.Code);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Category updated successfully",
                    Data = new
                    {
                        category.Id,
                        category.Code,
                        category.Name,
                        category.ParentId,
                        category.MapId,
                        category.Level,
                        category.Note,
                        category.Order,
                        category.Color,
                        category.Icon,
                        category.IsVisible,
                        category.Type,
                        category.Metadata,
                        category.IsActive,
                        category.CreatedAt,
                        category.UpdatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating category with ID: {CategoryId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while updating the category",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Category not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Category deleted with ID: {CategoryId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Category deleted successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while deleting category with ID: {CategoryId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while deleting the category",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets root categories (categories without parent)
        /// </summary>
        /// <returns>List of root categories</returns>
        [HttpGet("root")]
        public async Task<IActionResult> GetRootCategories()
        {
            try
            {
                var categories = await _categoryService.GetRootCategoriesAsync();
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Children = c.Children.Select(ch => new CategoryResponse
                    {
                        Id = ch.Id,
                        Code = ch.Code,
                        Name = ch.Name,
                        ParentId = ch.ParentId,
                        Level = ch.Level,
                        Note = ch.Note,
                        Order = ch.Order,
                        Color = ch.Color,
                        Icon = ch.Icon,
                        IsVisible = ch.IsVisible,
                        Type = ch.Type,
                        Metadata = ch.Metadata,
                        IsActive = ch.IsActive,
                        CreatedAt = ch.CreatedAt,
                        UpdatedAt = ch.UpdatedAt,
                        ChildrenCount = ch.Children.Count
                    }).ToList(),
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root categories");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving root categories",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets child categories of a specific parent
        /// </summary>
        /// <param name="parentId">Parent category ID</param>
        /// <returns>List of child categories</returns>
        [HttpGet("parent/{parentId}/children")]
        public async Task<IActionResult> GetChildCategories(int parentId)
        {
            try
            {
                var categories = await _categoryService.GetChildCategoriesAsync(parentId);
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting child categories for parent ID: {ParentId}", parentId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving child categories",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets category hierarchy (category and all its children)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category hierarchy</returns>
        [HttpGet("{id}/hierarchy")]
        public async Task<IActionResult> GetCategoryHierarchy(int id)
        {
            try
            {
                var categories = await _categoryService.GetCategoryHierarchyAsync(id);
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category hierarchy for ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving category hierarchy",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets category tree (visible categories only)
        /// </summary>
        /// <returns>Category tree</returns>
        [HttpGet("tree")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryTree()
        {
            try
            {
                var categories = await _categoryService.GetCategoryTreeAsync();
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Children = c.Children.Select(ch => new CategoryResponse
                    {
                        Id = ch.Id,
                        Code = ch.Code,
                        Name = ch.Name,
                        ParentId = ch.ParentId,
                        Level = ch.Level,
                        Note = ch.Note,
                        Order = ch.Order,
                        Color = ch.Color,
                        Icon = ch.Icon,
                        IsVisible = ch.IsVisible,
                        Type = ch.Type,
                        Metadata = ch.Metadata,
                        IsActive = ch.IsActive,
                        CreatedAt = ch.CreatedAt,
                        UpdatedAt = ch.UpdatedAt,
                        ChildrenCount = ch.Children.Count
                    }).ToList(),
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category tree");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving category tree",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets category path (breadcrumb)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category path</returns>
        [HttpGet("{id}/path")]
        public async Task<IActionResult> GetCategoryPath(int id)
        {
            try
            {
                var categories = await _categoryService.GetCategoryPathAsync(id);
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category path for ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving category path",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Reorders categories
        /// </summary>
        /// <param name="request">Reorder request</param>
        /// <returns>Reorder result</returns>
        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderCategories([FromBody] ReorderCategoriesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _categoryService.ReorderCategoriesAsync(request.ParentId ?? 0, request.CategoryIds);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to reorder categories",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Categories reordered for parent ID: {ParentId}", request.ParentId);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Categories reordered successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reordering categories for parent ID: {ParentId}", request.ParentId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while reordering categories",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Moves a category up in order
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Move result</returns>
        [HttpPost("{id}/move-up")]
        public async Task<IActionResult> MoveCategoryUp(int id)
        {
            try
            {
                var success = await _categoryService.MoveCategoryUpAsync(id);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to move category up",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Category moved up with ID: {CategoryId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Category moved up successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving category up with ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while moving category up",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Moves a category down in order
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Move result</returns>
        [HttpPost("{id}/move-down")]
        public async Task<IActionResult> MoveCategoryDown(int id)
        {
            try
            {
                var success = await _categoryService.MoveCategoryDownAsync(id);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to move category down",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Category moved down with ID: {CategoryId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Category moved down successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving category down with ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while moving category down",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Toggles category visibility
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Toggle result</returns>
        [HttpPost("{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleCategoryVisibility(int id)
        {
            try
            {
                var success = await _categoryService.ToggleCategoryVisibilityAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Category not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Category visibility toggled with ID: {CategoryId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Category visibility toggled successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling category visibility with ID: {CategoryId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while toggling category visibility",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Searches categories by term
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching categories</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories([FromQuery] string searchTerm)
        {
            try
            {
                var categories = await _categoryService.SearchCategoriesAsync(searchTerm);
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    MapId = c.MapId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    SearchTerm = searchTerm,
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching categories with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while searching categories",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets categories by level
        /// </summary>
        /// <param name="level">Category level</param>
        /// <returns>Categories at the specified level</returns>
        [HttpGet("by-level/{level}")]
        public async Task<IActionResult> GetCategoriesByLevel(int level)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesByLevelAsync(level);
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    MapId = c.MapId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Level = level,
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories by level: {Level}", level);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving categories by level",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets categories by type
        /// </summary>
        /// <param name="type">Category type</param>
        /// <returns>Categories of the specified type</returns>
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetCategoriesByType(string type)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesByTypeAsync(type);
                
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    MapId = c.MapId,
                    Level = c.Level,
                    Note = c.Note,
                    Order = c.Order,
                    Color = c.Color,
                    Icon = c.Icon,
                    IsVisible = c.IsVisible,
                    Type = c.Type,
                    Metadata = c.Metadata,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Type = type,
                    Count = categoryResponses.Count,
                    Data = categoryResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories by type: {Type}", type);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving categories by type",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a category by MapId
        /// </summary>
        /// <param name="mapId">Map ID</param>
        /// <returns>Category details</returns>
        [HttpGet("by-map-id/{mapId}")]
        public async Task<IActionResult> GetCategoryByMapId(string mapId)
        {
            try
            {
                var category = await _categoryService.GetCategoryByMapIdAsync(mapId);
                
                if (category == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Category not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Code = category.Code,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    MapId = category.MapId,
                    Level = category.Level,
                    Note = category.Note,
                    Order = category.Order,
                    Color = category.Color,
                    Icon = category.Icon,
                    IsVisible = category.IsVisible,
                    Type = category.Type,
                    Metadata = category.Metadata,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt,
                    ChildrenCount = category.Children.Count
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = categoryResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with MapId: {MapId}", mapId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the category",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Checks if a code is unique
        /// </summary>
        /// <param name="code">Category code</param>
        /// <param name="excludeId">Category ID to exclude from check</param>
        /// <returns>Uniqueness result</returns>
        [HttpGet("check-code-unique")]
        public async Task<IActionResult> CheckCodeUnique([FromQuery] string code, [FromQuery] int? excludeId = null)
        {
            try
            {
                var isUnique = await _categoryService.IsCodeUniqueAsync(code, excludeId);
                
                return Ok(new
                {
                    Status = "Success",
                    Code = code,
                    IsUnique = isUnique,
                    Message = isUnique ? "Code is available" : "Code already exists"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking code uniqueness: {Code}", code);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while checking code uniqueness",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Checks if a MapId is unique
        /// </summary>
        /// <param name="mapId">Map ID</param>
        /// <param name="excludeId">Category ID to exclude from check</param>
        /// <returns>Uniqueness result</returns>
        [HttpGet("check-map-id-unique")]
        public async Task<IActionResult> CheckMapIdUnique([FromQuery] string mapId, [FromQuery] int? excludeId = null)
        {
            try
            {
                var isUnique = await _categoryService.IsMapIdUniqueAsync(mapId, excludeId);
                
                return Ok(new
                {
                    Status = "Success",
                    MapId = mapId,
                    IsUnique = isUnique,
                    Message = isUnique ? "MapId is available" : "MapId already exists"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking MapId uniqueness: {MapId}", mapId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while checking MapId uniqueness",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
