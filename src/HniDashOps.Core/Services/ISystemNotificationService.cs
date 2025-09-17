using HniDashOps.Core.Entities;

namespace HniDashOps.Core.Services
{
    public interface ISystemNotificationService
    {
        // Basic CRUD operations
        Task<IEnumerable<SystemNotification>> GetNotificationsAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<SystemNotification?> GetNotificationByIdAsync(int id);
        Task<SystemNotification> CreateNotificationAsync(SystemNotification notification);
        Task<SystemNotification> UpdateNotificationAsync(SystemNotification notification);
        Task<bool> DeleteNotificationAsync(int id);

        // Status operations
        Task<bool> MarkAsReadAsync(int id, int userId);
        Task<bool> MarkAsUnreadAsync(int id);
        Task<IEnumerable<SystemNotification>> GetUnreadNotificationsAsync(int userId);

        // Type and Status filtering
        Task<IEnumerable<SystemNotification>> GetNotificationsByTypeAsync(string type, int page = 1, int pageSize = 10);
        Task<IEnumerable<SystemNotification>> GetNotificationsByStatusAsync(string status, int page = 1, int pageSize = 10);

        // Time-based operations
        Task<IEnumerable<SystemNotification>> GetActiveNotificationsAsync();
        Task<IEnumerable<SystemNotification>> GetNotificationsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Priority operations
        Task<IEnumerable<SystemNotification>> GetHighPriorityNotificationsAsync();
        Task<bool> UpdatePriorityAsync(int id, string priority);

        // Target audience operations
        Task<IEnumerable<SystemNotification>> GetNotificationsByAudienceAsync(string audience, int page = 1, int pageSize = 10);

        // Bulk operations
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteExpiredNotificationsAsync();
        Task<int> GetUnreadCountAsync(int userId);

        // Validation
        Task<bool> IsValidNotificationAsync(SystemNotification notification);
        Task<bool> IsNotificationActiveAsync(int id);
    }
}
