using HniDashOps.Core.Entities;
using HniDashOps.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace HniDashOps.Core.Services;

/// <summary>
/// Base service class that provides common functionality for audit fields
/// </summary>
public abstract class BaseService
{
    protected readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Sets the audit fields for entity creation
    /// </summary>
    /// <param name="entity">The entity to set audit fields for</param>
    protected void SetCreatedAuditFields(BaseEntity entity)
    {
        var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
        
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = currentUserId;
        entity.UpdatedAt = null;
        entity.UpdatedBy = null;
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.DeletedBy = null;
    }

    /// <summary>
    /// Sets the audit fields for entity update
    /// </summary>
    /// <param name="entity">The entity to set audit fields for</param>
    protected void SetUpdatedAuditFields(BaseEntity entity)
    {
        var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
        
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = currentUserId;
    }

    /// <summary>
    /// Sets the audit fields for entity deletion (soft delete)
    /// </summary>
    /// <param name="entity">The entity to set audit fields for</param>
    protected void SetDeletedAuditFields(BaseEntity entity)
    {
        var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
        
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = currentUserId;
    }
}
