using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using HniDashOps.Core.Authorization;
using HniDashOps.Core.Extensions;
using HniDashOps.Core.Entities;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Authorization;

/// <summary>
/// Authorization handler cho resource-based authorization
/// </summary>
public class ResourceAuthorizationHandler : AuthorizationHandler<AuthorizeResourceAttribute>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    
    public ResourceAuthorizationHandler(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        AuthorizeResourceAttribute requirement)
    {
        var userId = context.User.GetUserId();
        
        // Check SuperAdmin bypass first
        var userRole = await GetUserRole(userId);
        if (userRole == UserRole.SuperAdmin)
        {
            context.Succeed(requirement); // SuperAdmin bypass tất cả
            return;
        }
        
        // Check normal authorization
        var typeDataId = requirement.TypeDataId;
        var resourceIds = requirement.ResourceIds;
        var logic = requirement.Logic;
        
        var userResourceIds = await GetUserAccessibleResourceIds(userId, typeDataId);
        
        bool hasAccess = logic switch
        {
            LogicType.AND => resourceIds.All(id => userResourceIds.Contains(id)),
            LogicType.OR => resourceIds.Any(id => userResourceIds.Contains(id)),
            _ => false
        };
        
        if (hasAccess)
        {
            context.Succeed(requirement);
        }
    }
    
    private async Task<UserRole> GetUserRole(int userId)
    {
        var cacheKey = $"user_role_{userId}";
        
        if (!_cache.TryGetValue(cacheKey, out UserRole role))
        {
            role = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.RoleId)
                .FirstOrDefaultAsync();
                
            _cache.Set(cacheKey, role, TimeSpan.FromMinutes(15));
        }
        
        return role;
    }
    
    private async Task<HashSet<int>> GetUserAccessibleResourceIds(int userId, AuthorizationType typeDataId)
    {
        var cacheKey = $"user_{typeDataId}_resources_{userId}";
        
        if (!_cache.TryGetValue(cacheKey, out HashSet<int> resourceIds))
        {
            resourceIds = typeDataId switch
            {
                AuthorizationType.Menu => await GetUserMenuIds(userId),
                // Future-ready - comment out for now
                // AuthorizationType.Department => await GetUserDepartmentIds(userId),
                _ => new HashSet<int>()
            };
            
            _cache.Set(cacheKey, resourceIds, TimeSpan.FromMinutes(15));
        }
        
        return resourceIds;
    }
    
    private async Task<HashSet<int>> GetUserMenuIds(int userId)
    {
        return await _context.GroupUsers
            .Where(gu => gu.UserId == userId && gu.IsActive)
            .SelectMany(gu => gu.Group.GroupMenus
                .Where(gm => gm.IsActive)
                .Select(gm => gm.MenuId))
            .ToHashSetAsync();
    }
    
    // Future-ready method - comment out for now
    // private async Task<HashSet<int>> GetUserDepartmentIds(int userId)
    // {
    //     return await _context.UserGroups
    //         .Where(ug => ug.UserId == userId && ug.IsActive)
    //         .SelectMany(ug => ug.Group.GroupDepartments
    //             .Where(gd => gd.IsActive)
    //             .Select(gd => gd.DepartmentId))
    //         .ToHashSetAsync();
    // }
}

