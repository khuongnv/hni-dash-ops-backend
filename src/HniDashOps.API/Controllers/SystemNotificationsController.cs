using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HniDashOps.Core.Entities;
using HniDashOps.Core.Services;
using HniDashOps.Core.Authorization;
using HniDashOps.API.DTOs;

namespace HniDashOps.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeSuperAdmin]
    public class SystemNotificationsController : ControllerBase
    {
        private readonly ISystemNotificationService _notificationService;
        private readonly ILogger<SystemNotificationsController> _logger;

        public SystemNotificationsController(ISystemNotificationService notificationService, ILogger<SystemNotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách thông báo hệ thống
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsAsync(page, pageSize, search);
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemNotificationResponse>> GetNotification(int id)
        {
            try
            {
                var notification = await _notificationService.GetNotificationByIdAsync(id);
                if (notification == null)
                {
                    return NotFound();
                }

                return Ok(MapToResponse(notification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification by id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tạo thông báo mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SystemNotificationResponse>> CreateNotification([FromBody] CreateSystemNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var notification = new SystemNotification
                {
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type,
                    Status = request.Status,
                    StartAt = request.StartAt,
                    EndAt = request.EndAt,
                    Priority = request.Priority,
                    TargetAudience = request.TargetAudience,
                    ActionUrl = request.ActionUrl,
                    ActionText = request.ActionText,
                    Metadata = request.Metadata,
                    IsActive = request.IsActive
                };

                var isValid = await _notificationService.IsValidNotificationAsync(notification);
                if (!isValid)
                {
                    return BadRequest("Invalid notification data");
                }

                var createdNotification = await _notificationService.CreateNotificationAsync(notification);
                return CreatedAtAction(nameof(GetNotification), new { id = createdNotification.Id }, MapToResponse(createdNotification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cập nhật thông báo
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<SystemNotificationResponse>> UpdateNotification(int id, [FromBody] UpdateSystemNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingNotification = await _notificationService.GetNotificationByIdAsync(id);
                if (existingNotification == null)
                {
                    return NotFound();
                }

                existingNotification.Title = request.Title;
                existingNotification.Message = request.Message;
                existingNotification.Type = request.Type;
                existingNotification.Status = request.Status;
                existingNotification.StartAt = request.StartAt;
                existingNotification.EndAt = request.EndAt;
                existingNotification.Priority = request.Priority;
                existingNotification.TargetAudience = request.TargetAudience;
                existingNotification.ActionUrl = request.ActionUrl;
                existingNotification.ActionText = request.ActionText;
                existingNotification.Metadata = request.Metadata;
                existingNotification.IsActive = request.IsActive;

                var isValid = await _notificationService.IsValidNotificationAsync(existingNotification);
                if (!isValid)
                {
                    return BadRequest("Invalid notification data");
                }

                var updatedNotification = await _notificationService.UpdateNotificationAsync(existingNotification);
                return Ok(MapToResponse(updatedNotification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            try
            {
                var result = await _notificationService.DeleteNotificationAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Đánh dấu thông báo đã đọc
        /// </summary>
        [HttpPost("{id}/mark-read")]
        public async Task<ActionResult> MarkAsRead(int id, [FromBody] MarkNotificationReadRequest request)
        {
            try
            {
                var result = await _notificationService.MarkAsReadAsync(id, request.UserId);
                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Đánh dấu thông báo chưa đọc
        /// </summary>
        [HttpPost("{id}/mark-unread")]
        public async Task<ActionResult> MarkAsUnread(int id)
        {
            try
            {
                var result = await _notificationService.MarkAsUnreadAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as unread");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo chưa đọc
        /// </summary>
        [HttpGet("unread/{userId}")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetUnreadNotifications(int userId)
        {
            try
            {
                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo theo loại
        /// </summary>
        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetNotificationsByType(
            string type,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByTypeAsync(type, page, pageSize);
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by type: {Type}", type);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo theo trạng thái
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetNotificationsByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByStatusAsync(status, page, pageSize);
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by status: {Status}", status);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo đang hoạt động
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetActiveNotifications()
        {
            try
            {
                var notifications = await _notificationService.GetActiveNotificationsAsync();
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo theo khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetNotificationsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByDateRangeAsync(startDate, endDate);
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by date range");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo ưu tiên cao
        /// </summary>
        [HttpGet("high-priority")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetHighPriorityNotifications()
        {
            try
            {
                var notifications = await _notificationService.GetHighPriorityNotificationsAsync();
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting high priority notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cập nhật độ ưu tiên thông báo
        /// </summary>
        [HttpPut("{id}/priority")]
        public async Task<ActionResult> UpdatePriority(int id, [FromBody] UpdateNotificationPriorityRequest request)
        {
            try
            {
                var result = await _notificationService.UpdatePriorityAsync(id, request.Priority);
                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification priority");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thông báo theo đối tượng
        /// </summary>
        [HttpGet("by-audience/{audience}")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponse>>> GetNotificationsByAudience(
            string audience,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByAudienceAsync(audience, page, pageSize);
                var response = notifications.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by audience: {Audience}", audience);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo đã đọc
        /// </summary>
        [HttpPost("mark-all-read/{userId}")]
        public async Task<ActionResult> MarkAllAsRead(int userId)
        {
            try
            {
                var result = await _notificationService.MarkAllAsReadAsync(userId);
                if (!result)
                {
                    return BadRequest("Failed to mark all notifications as read");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Xóa thông báo hết hạn
        /// </summary>
        [HttpDelete("expired")]
        public async Task<ActionResult> DeleteExpiredNotifications()
        {
            try
            {
                var result = await _notificationService.DeleteExpiredNotificationsAsync();
                if (!result)
                {
                    return BadRequest("Failed to delete expired notifications");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expired notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        [HttpGet("unread-count/{userId}")]
        public async Task<ActionResult<int>> GetUnreadCount(int userId)
        {
            try
            {
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Kiểm tra thông báo có đang hoạt động không
        /// </summary>
        [HttpGet("{id}/is-active")]
        public async Task<ActionResult<bool>> IsNotificationActive(int id)
        {
            try
            {
                var isActive = await _notificationService.IsNotificationActiveAsync(id);
                return Ok(isActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if notification is active: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private static SystemNotificationResponse MapToResponse(SystemNotification notification)
        {
            return new SystemNotificationResponse
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Status = notification.Status,
                StartAt = notification.StartAt,
                EndAt = notification.EndAt,
                Priority = notification.Priority,
                TargetAudience = notification.TargetAudience,
                ActionUrl = notification.ActionUrl,
                ActionText = notification.ActionText,
                IsRead = notification.IsRead,
                ReadAt = notification.ReadAt,
                ReadBy = notification.ReadBy,
                Metadata = notification.Metadata,
                IsActive = notification.IsActive,
                CreatedAt = notification.CreatedAt,
                UpdatedAt = notification.UpdatedAt
            };
        }
    }
}
