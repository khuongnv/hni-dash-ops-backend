using Microsoft.AspNetCore.Authorization;

namespace HniDashOps.Core.Authorization;

/// <summary>
/// Authorization attribute cho Menu với logic OR (chỉ cần có 1 menu)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeMenuAttribute : AuthorizeResourceAttribute
{
    public AuthorizeMenuAttribute(params int[] menuIds) 
        : base(AuthorizationType.Menu, menuIds, LogicType.OR) { }
}

/// <summary>
/// Authorization attribute cho Menu với logic AND (phải có tất cả menus)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAllMenusAttribute : AuthorizeResourceAttribute
{
    public AuthorizeAllMenusAttribute(params int[] menuIds) 
        : base(AuthorizationType.Menu, menuIds, LogicType.AND) { }
}

// Future-ready attributes - comment out for now
// [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
// public class AuthorizeDepartmentAttribute : AuthorizeResourceAttribute
// {
//     public AuthorizeDepartmentAttribute(params int[] departmentIds) 
//         : base(AuthorizationType.Department, departmentIds, LogicType.OR) { }
// }
//
// [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
// public class AuthorizeAllDepartmentsAttribute : AuthorizeResourceAttribute
// {
//     public AuthorizeAllDepartmentsAttribute(params int[] departmentIds) 
//         : base(AuthorizationType.Department, departmentIds, LogicType.AND) { }
// }

