using Microsoft.AspNetCore.Authorization;
using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Authorization;

/// <summary>
/// Base authorization attribute cho role-based authorization
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationRequirement
{
    public UserRole[] AllowedRoles { get; }
    
    public AuthorizeRoleAttribute(params UserRole[] allowedRoles)
    {
        AllowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
    }
}

/// <summary>
/// Authorization attribute cho Admin roles (SuperAdmin và SubAdmin)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAdminAttribute : AuthorizeRoleAttribute
{
    public AuthorizeAdminAttribute() : base(UserRole.SuperAdmin, UserRole.SubAdmin) { }
}

/// <summary>
/// Authorization attribute chỉ cho SuperAdmin
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeSuperAdminAttribute : AuthorizeRoleAttribute
{
    public AuthorizeSuperAdminAttribute() : base(UserRole.SuperAdmin) { }
}

/// <summary>
/// Authorization attribute cho Member trở lên (SuperAdmin, SubAdmin, Member)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeMemberOrAboveAttribute : AuthorizeRoleAttribute
{
    public AuthorizeMemberOrAboveAttribute() : base(UserRole.SuperAdmin, UserRole.SubAdmin, UserRole.Member) { }
}

/// <summary>
/// Authorization attribute cho SubAdmin trở lên (SuperAdmin, SubAdmin)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeSubAdminOrAboveAttribute : AuthorizeRoleAttribute
{
    public AuthorizeSubAdminOrAboveAttribute() : base(UserRole.SuperAdmin, UserRole.SubAdmin) { }
}

