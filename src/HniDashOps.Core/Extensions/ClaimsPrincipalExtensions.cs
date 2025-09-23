using System.Security.Claims;

namespace HniDashOps.Core.Extensions;

/// <summary>
/// Extension methods cho ClaimsPrincipal
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Lấy User ID từ JWT claims
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? 
                         principal.FindFirst("sub") ?? 
                         principal.FindFirst("user_id");
        
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        
        throw new UnauthorizedAccessException("User ID not found in claims");
    }
    
    /// <summary>
    /// Lấy Username từ JWT claims
    /// </summary>
    public static string GetUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value ?? 
               principal.FindFirst("username")?.Value ?? 
               throw new UnauthorizedAccessException("Username not found in claims");
    }
    
    /// <summary>
    /// Lấy Email từ JWT claims
    /// </summary>
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ?? 
               principal.FindFirst("email")?.Value ?? 
               throw new UnauthorizedAccessException("Email not found in claims");
    }
}

