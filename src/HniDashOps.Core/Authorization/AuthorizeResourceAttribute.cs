using Microsoft.AspNetCore.Authorization;

namespace HniDashOps.Core.Authorization;

/// <summary>
/// Base authorization attribute cho resource-based authorization
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeResourceAttribute : Attribute, IAuthorizationRequirement
{
    public AuthorizationType TypeDataId { get; }
    public int[] ResourceIds { get; }
    public LogicType Logic { get; }
    
    public AuthorizeResourceAttribute(AuthorizationType typeDataId, int[] resourceIds, LogicType logic = LogicType.OR)
    {
        TypeDataId = typeDataId;
        ResourceIds = resourceIds ?? throw new ArgumentNullException(nameof(resourceIds));
        Logic = logic;
    }
}

