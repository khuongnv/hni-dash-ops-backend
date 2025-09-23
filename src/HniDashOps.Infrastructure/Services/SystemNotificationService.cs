using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Infrastructure.Data;

namespace HniDashOps.Infrastructure.Services
{
    public class SystemNotificationService : BaseService, ISystemNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemNotificationService> _logger;

        public SystemNotificationService(ApplicationDbContext context, ILogger<SystemNotificationService> logger, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SystemNotification>> GetNotificationsAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            try
            {
                var query = _context.SystemNotifications
                    .Where(n => !n.IsDeleted);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(n => n.Title.Contains(search) || n.Message.Contains(search));
                }

                query = query.OrderByDescending(n => n.CreatedAt);

                return await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                throw;
            }
        }

        public async Task<SystemNotification?> GetNotificationByIdAsync(int id)
        {
            try
            {
                return await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification by id: {Id}", id);
                throw;
            }
        }

        public async Task<SystemNotification> CreateNotificationAsync(SystemNotification notification)
        {
            try
            {
                notification.IsActive = true;
                notification.IsDeleted = false;

                // Set audit fields for creation
                SetCreatedAuditFields(notification);

                _context.SystemNotifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created notification with id: {Id}", notification.Id);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                throw;
            }
        }

        public async Task<SystemNotification> UpdateNotificationAsync(SystemNotification notification)
        {
            try
            {
                var existingNotification = await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == notification.Id && !n.IsDeleted);

                if (existingNotification == null)
                {
                    throw new ArgumentException("Notification not found");
                }

                existingNotification.Title = notification.Title;
                existingNotification.Message = notification.Message;
                existingNotification.Type = notification.Type;
                existingNotification.Status = notification.Status;
                existingNotification.StartAt = notification.StartAt;
                existingNotification.EndAt = notification.EndAt;
                existingNotification.Priority = notification.Priority;
                existingNotification.TargetAudience = notification.TargetAudience;
                existingNotification.ActionUrl = notification.ActionUrl;
                existingNotification.ActionText = notification.ActionText;
                existingNotification.Metadata = notification.Metadata;
                existingNotification.IsActive = notification.IsActive;

                // Set audit fields for update
                SetUpdatedAuditFields(existingNotification);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated notification with id: {Id}", notification.Id);
                return existingNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification");
                throw;
            }
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            try
            {
                var notification = await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

                if (notification == null)
                {
                    return false;
                }

                // Soft delete
                SetDeletedAuditFields(notification);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted notification with id: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                throw;
            }
        }

        public async Task<bool> MarkAsReadAsync(int id, int userId)
        {
            try
            {
                var notification = await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

                if (notification == null)
                {
                    return false;
                }

                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                notification.ReadBy = userId;
                notification.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Marked notification {Id} as read by user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                throw;
            }
        }

        public async Task<bool> MarkAsUnreadAsync(int id)
        {
            try
            {
                var notification = await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

                if (notification == null)
                {
                    return false;
                }

                notification.IsRead = false;
                notification.ReadAt = null;
                notification.ReadBy = null;
                notification.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Marked notification {Id} as unread", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as unread");
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetUnreadNotificationsAsync(int userId)
        {
            try
            {
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && !n.IsRead && n.IsActive)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetNotificationsByTypeAsync(string type, int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && n.Type == type)
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by type: {Type}", type);
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetNotificationsByStatusAsync(string status, int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && n.Status == status)
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetActiveNotificationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && n.IsActive && 
                               (n.StartAt == null || n.StartAt <= now) &&
                               (n.EndAt == null || n.EndAt >= now))
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active notifications");
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetNotificationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && n.CreatedAt >= startDate && n.CreatedAt <= endDate)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by date range");
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetHighPriorityNotificationsAsync()
        {
            try
            {
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && n.Priority == "HIGH" && n.IsActive)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting high priority notifications");
                throw;
            }
        }

        public async Task<bool> UpdatePriorityAsync(int id, string priority)
        {
            try
            {
                var notification = await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

                if (notification == null)
                {
                    return false;
                }

                notification.Priority = priority;
                notification.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated priority for notification {Id} to {Priority}", id, priority);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification priority");
                throw;
            }
        }

        public async Task<IEnumerable<SystemNotification>> GetNotificationsByAudienceAsync(string audience, int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && (n.TargetAudience == audience || n.TargetAudience == "ALL"))
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by audience: {Audience}", audience);
                throw;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var notifications = await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    notification.ReadBy = userId;
                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Marked all notifications as read for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                throw;
            }
        }

        public async Task<bool> DeleteExpiredNotificationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var expiredNotifications = await _context.SystemNotifications
                    .Where(n => !n.IsDeleted && n.EndAt != null && n.EndAt < now)
                    .ToListAsync();

                foreach (var notification in expiredNotifications)
                {
                    notification.IsDeleted = true;
                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted {Count} expired notifications", expiredNotifications.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expired notifications");
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                return await _context.SystemNotifications
                    .CountAsync(n => !n.IsDeleted && !n.IsRead && n.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                throw;
            }
        }

        public Task<bool> IsValidNotificationAsync(SystemNotification notification)
        {
            try
            {
                if (string.IsNullOrEmpty(notification.Title) || string.IsNullOrEmpty(notification.Message))
                {
                    return Task.FromResult(false);
                }

                if (notification.StartAt.HasValue && notification.EndAt.HasValue && 
                    notification.StartAt > notification.EndAt)
                {
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating notification");
                return Task.FromResult(false);
            }
        }

        public async Task<bool> IsNotificationActiveAsync(int id)
        {
            try
            {
                var notification = await _context.SystemNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

                if (notification == null)
                {
                    return false;
                }

                var now = DateTime.UtcNow;
                return notification.IsActive && 
                       (notification.StartAt == null || notification.StartAt <= now) &&
                       (notification.EndAt == null || notification.EndAt >= now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if notification is active");
                return false;
            }
        }
    }
}
