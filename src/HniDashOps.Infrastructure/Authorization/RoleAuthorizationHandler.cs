using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using HniDashOps.Core.Authorization;
using HniDashOps.Core.Extensions;
using HniDashOps.Core.Entities;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Authorization;

/// <summary>
/// Authorization handler cho role-based authorization
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<AuthorizeRoleAttribute>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    
    public RoleAuthorizationHandler(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        AuthorizeRoleAttribute requirement)
    {
        var userId = context.User.GetUserId();
        var userRole = await GetUserRole(userId);
        
        if (requirement.AllowedRoles.Contains(userRole))
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
}

