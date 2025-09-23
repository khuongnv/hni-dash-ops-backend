namespace HniDashOps.Core.Authorization;

/// <summary>
/// Enum định nghĩa các loại resource trong hệ thống authorization
/// </summary>
public enum AuthorizationType
{
    /// <summary>
    /// Menu-based authorization
    /// </summary>
    Menu = 1,
    
    // Future-ready - comment out for now
    // Department = 2,
    // Category = 3,
    // Role = 4,
    // Feature = 5,
    // Project = 6,
    // Client = 7,
    // etc.
}

/// <summary>
/// Enum định nghĩa logic cho authorization
/// </summary>
public enum LogicType
{
    /// <summary>
    /// Phải có TẤT CẢ resource IDs
    /// </summary>
    AND,
    
    /// <summary>
    /// Chỉ cần có 1 resource ID
    /// </summary>
    OR
}

