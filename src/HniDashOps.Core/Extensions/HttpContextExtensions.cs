using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HniDashOps.Core.Extensions;

/// <summary>
/// Extension methods for HttpContext
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the current user ID from the HTTP context claims
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>The user ID if found, otherwise null</returns>
    public static int? GetCurrentUserId(this HttpContext httpContext)
    {
        var userIdClaim = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return null;
            
        if (int.TryParse(userIdClaim, out int userId))
            return userId;
            
        return null;
    }

    /// <summary>
    /// Gets the current user ID from the HTTP context claims
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>The user ID if found, otherwise 0</returns>
    public static int GetCurrentUserIdOrDefault(this HttpContext httpContext)
    {
        return httpContext.GetCurrentUserId() ?? 0;
    }
}
