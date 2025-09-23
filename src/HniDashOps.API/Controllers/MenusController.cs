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
    /// Controller for managing menus
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeSuperAdmin]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenusController> _logger;

        public MenusController(IMenuService menuService, ILogger<MenusController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all menus
        /// </summary>
        /// <returns>List of menus</returns>
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                var menus = await _menuService.GetAllMenusAsync();
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    ParentName = m.Parent?.Name,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menus");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving menus",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a specific menu by ID
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Menu details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(int id)
        {
            try
            {
                var menu = await _menuService.GetMenuByIdAsync(id);
                
                if (menu == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Menu not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var menuResponse = new MenuResponse
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Href = menu.Href,
                    Icon = menu.Icon,
                    Order = menu.Order,
                    ParentId = menu.ParentId,
                    ParentName = menu.Parent?.Name,
                    Level = menu.Level,
                    Description = menu.Description,
                    IsVisible = menu.IsVisible,
                    Target = menu.Target,
                    CssClass = menu.CssClass,
                    DataAttributes = menu.DataAttributes,
                    IsActive = menu.IsActive,
                    CreatedAt = menu.CreatedAt,
                    UpdatedAt = menu.UpdatedAt,
                    Children = menu.Children.Select(c => new MenuResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Href = c.Href,
                        Icon = c.Icon,
                        Order = c.Order,
                        ParentId = c.ParentId,
                        Level = c.Level,
                        Description = c.Description,
                        IsVisible = c.IsVisible,
                        Target = c.Target,
                        CssClass = c.CssClass,
                        DataAttributes = c.DataAttributes,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }).ToList()
                };

                return Ok(new
                {
                    Status = "Success",
                    Data = menuResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu with ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving the menu",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new menu
        /// </summary>
        /// <param name="request">Menu creation request</param>
        /// <returns>Created menu</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var menu = await _menuService.CreateMenuAsync(
                    request.Name,
                    request.Href,
                    request.Icon,
                    request.Order,
                    request.ParentId,
                    request.Description,
                    request.IsVisible,
                    request.Target,
                    request.CssClass,
                    request.DataAttributes);

                _logger.LogInformation("New menu created: {Name} with ID: {MenuId}", menu.Name, menu.Id);

                return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, new
                {
                    Status = "Success",
                    Message = "Menu created successfully",
                    Data = new
                    {
                        menu.Id,
                        menu.Name,
                        menu.Href,
                        menu.Icon,
                        menu.Order,
                        menu.ParentId,
                        menu.Level,
                        menu.Description,
                        menu.IsVisible,
                        menu.Target,
                        menu.CssClass,
                        menu.DataAttributes,
                        menu.IsActive,
                        menu.CreatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating menu: {Name}", request.Name);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating menu: {Name}", request.Name);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while creating the menu",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates an existing menu
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <param name="request">Menu update request</param>
        /// <returns>Updated menu</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var menu = await _menuService.UpdateMenuAsync(
                    id,
                    request.Name,
                    request.Href,
                    request.Icon,
                    request.Order,
                    request.ParentId,
                    request.Level,
                    request.Description,
                    request.IsVisible,
                    request.Target,
                    request.CssClass,
                    request.DataAttributes);
                
                if (menu == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Menu not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Menu updated: {Name} with ID: {MenuId}", menu.Name, menu.Id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Menu updated successfully",
                    Data = new
                    {
                        menu.Id,
                        menu.Name,
                        menu.Href,
                        menu.Icon,
                        menu.Order,
                        menu.ParentId,
                        menu.Level,
                        menu.Description,
                        menu.IsVisible,
                        menu.Target,
                        menu.CssClass,
                        menu.DataAttributes,
                        menu.IsActive,
                        menu.CreatedAt,
                        menu.UpdatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating menu with ID: {MenuId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating menu with ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while updating the menu",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a menu
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            try
            {
                var success = await _menuService.DeleteMenuAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Menu not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Menu deleted with ID: {MenuId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Menu deleted successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while deleting menu with ID: {MenuId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting menu with ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while deleting the menu",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets root menus (menus without parent)
        /// </summary>
        /// <returns>List of root menus</returns>
        [HttpGet("root")]
        public async Task<IActionResult> GetRootMenus()
        {
            try
            {
                var menus = await _menuService.GetRootMenusAsync();
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Children = m.Children.Select(c => new MenuResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Href = c.Href,
                        Icon = c.Icon,
                        Order = c.Order,
                        ParentId = c.ParentId,
                        Level = c.Level,
                        Description = c.Description,
                        IsVisible = c.IsVisible,
                        Target = c.Target,
                        CssClass = c.CssClass,
                        DataAttributes = c.DataAttributes,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root menus");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving root menus",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets child menus of a specific parent
        /// </summary>
        /// <param name="parentId">Parent menu ID</param>
        /// <returns>List of child menus</returns>
        [HttpGet("parent/{parentId}/children")]
        public async Task<IActionResult> GetChildMenus(int parentId)
        {
            try
            {
                var menus = await _menuService.GetChildMenusAsync(parentId);
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting child menus for parent ID: {ParentId}", parentId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving child menus",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets menu hierarchy (menu and all its children)
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Menu hierarchy</returns>
        [HttpGet("{id}/hierarchy")]
        public async Task<IActionResult> GetMenuHierarchy(int id)
        {
            try
            {
                var menus = await _menuService.GetMenuHierarchyAsync(id);
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    ParentName = m.Parent?.Name,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu hierarchy for ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving menu hierarchy",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets visible menus for navigation
        /// </summary>
        /// <returns>List of visible menus</returns>
        [HttpGet("navigation")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNavigationMenu()
        {
            try
            {
                var menus = await _menuService.GetNavigationMenuAsync();
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Children = m.Children.Select(c => new MenuResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Href = c.Href,
                        Icon = c.Icon,
                        Order = c.Order,
                        ParentId = c.ParentId,
                        Level = c.Level,
                        Description = c.Description,
                        IsVisible = c.IsVisible,
                        Target = c.Target,
                        CssClass = c.CssClass,
                        DataAttributes = c.DataAttributes,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting navigation menu");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving navigation menu",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets breadcrumb for a specific menu
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Breadcrumb path</returns>
        [HttpGet("{id}/breadcrumb")]
        public async Task<IActionResult> GetBreadcrumb(int id)
        {
            try
            {
                var menus = await _menuService.GetBreadcrumbAsync(id);
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting breadcrumb for menu ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving breadcrumb",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Reorders menus
        /// </summary>
        /// <param name="request">Reorder request</param>
        /// <returns>Reorder result</returns>
        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderMenus([FromBody] ReorderMenusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _menuService.ReorderMenusAsync(request.ParentId ?? 0, request.MenuIds);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to reorder menus",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Menus reordered for parent ID: {ParentId}", request.ParentId);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Menus reordered successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reordering menus for parent ID: {ParentId}", request.ParentId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while reordering menus",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Moves a menu up in order
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Move result</returns>
        [HttpPost("{id}/move-up")]
        public async Task<IActionResult> MoveMenuUp(int id)
        {
            try
            {
                var success = await _menuService.MoveMenuUpAsync(id);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to move menu up",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Menu moved up with ID: {MenuId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Menu moved up successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving menu up with ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while moving menu up",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Moves a menu down in order
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Move result</returns>
        [HttpPost("{id}/move-down")]
        public async Task<IActionResult> MoveMenuDown(int id)
        {
            try
            {
                var success = await _menuService.MoveMenuDownAsync(id);
                
                if (!success)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to move menu down",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Menu moved down with ID: {MenuId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Menu moved down successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving menu down with ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while moving menu down",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Toggles menu visibility
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Toggle result</returns>
        [HttpPost("{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleMenuVisibility(int id)
        {
            try
            {
                var success = await _menuService.ToggleMenuVisibilityAsync(id);
                
                if (!success)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Menu not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Menu visibility toggled with ID: {MenuId}", id);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Menu visibility toggled successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling menu visibility with ID: {MenuId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while toggling menu visibility",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Searches menus by term
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching menus</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchMenus([FromQuery] string searchTerm)
        {
            try
            {
                var menus = await _menuService.SearchMenusAsync(searchTerm);
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    ParentName = m.Parent?.Name,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    SearchTerm = searchTerm,
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching menus with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while searching menus",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets menus by level
        /// </summary>
        /// <param name="level">Menu level</param>
        /// <returns>Menus at the specified level</returns>
        [HttpGet("by-level/{level}")]
        public async Task<IActionResult> GetMenusByLevel(int level)
        {
            try
            {
                var menus = await _menuService.GetMenusByLevelAsync(level);
                
                var menuResponses = menus.Select(m => new MenuResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Href = m.Href,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    ParentName = m.Parent?.Name,
                    Level = m.Level,
                    Description = m.Description,
                    IsVisible = m.IsVisible,
                    Target = m.Target,
                    CssClass = m.CssClass,
                    DataAttributes = m.DataAttributes,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new
                {
                    Status = "Success",
                    Level = level,
                    Count = menuResponses.Count,
                    Data = menuResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menus by level: {Level}", level);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "An error occurred while retrieving menus by level",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
