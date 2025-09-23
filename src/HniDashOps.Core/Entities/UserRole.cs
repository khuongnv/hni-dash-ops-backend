namespace HniDashOps.Core.Entities;

/// <summary>
/// Enum định nghĩa các vai trò người dùng trong hệ thống
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Super Administrator - Có toàn quyền truy cập hệ thống
    /// </summary>
    SuperAdmin = 1,
    
    /// <summary>
    /// Sub Administrator - Có quyền quản trị hạn chế
    /// </summary>
    SubAdmin = 2,
    
    /// <summary>
    /// Member - Thành viên thông thường
    /// </summary>
    Member = 3,
    
    /// <summary>
    /// Guest - Khách truy cập với quyền hạn tối thiểu
    /// </summary>
    Guest = 4
}

/// <summary>
/// Extension methods cho UserRole enum
/// </summary>
public static class UserRoleExtensions
{
    /// <summary>
    /// Kiểm tra xem role có phải là Admin không (SuperAdmin hoặc SubAdmin)
    /// </summary>
    public static bool IsAdmin(this UserRole role) => role <= UserRole.SubAdmin;
    
    /// <summary>
    /// Kiểm tra xem role có phải là Member trở lên không
    /// </summary>
    public static bool IsMemberOrAbove(this UserRole role) => role <= UserRole.Member;
    
    /// <summary>
    /// Kiểm tra xem role có phải là Guest không
    /// </summary>
    public static bool IsGuest(this UserRole role) => role == UserRole.Guest;
    
    /// <summary>
    /// Lấy tên hiển thị của role
    /// </summary>
    public static string GetDisplayName(this UserRole role) => role switch
    {
        UserRole.SuperAdmin => "Super Administrator",
        UserRole.SubAdmin => "Sub Administrator",
        UserRole.Member => "Member",
        UserRole.Guest => "Guest",
        _ => "Unknown"
    };
}