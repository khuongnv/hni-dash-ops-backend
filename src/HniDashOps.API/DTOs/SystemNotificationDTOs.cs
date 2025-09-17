using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.DTOs
{
    public class SystemNotificationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? Priority { get; set; }
        public string? TargetAudience { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? ReadBy { get; set; }
        public string? Metadata { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSystemNotificationRequest
    {
        [Required, StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000, MinimumLength = 2)]
        public string Message { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        [StringLength(20)]
        public string? Priority { get; set; }
        [StringLength(100)]
        public string? TargetAudience { get; set; }
        [StringLength(500)]
        public string? ActionUrl { get; set; }
        [StringLength(100)]
        public string? ActionText { get; set; }
        [StringLength(2000)]
        public string? Metadata { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSystemNotificationRequest
    {
        [Required, StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000, MinimumLength = 2)]
        public string Message { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        [StringLength(20)]
        public string? Priority { get; set; }
        [StringLength(100)]
        public string? TargetAudience { get; set; }
        [StringLength(500)]
        public string? ActionUrl { get; set; }
        [StringLength(100)]
        public string? ActionText { get; set; }
        [StringLength(2000)]
        public string? Metadata { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class MarkNotificationReadRequest
    {
        [Required]
        public int UserId { get; set; }
    }

    public class UpdateNotificationPriorityRequest
    {
        [Required, StringLength(20)]
        public string Priority { get; set; } = string.Empty;
    }
}
