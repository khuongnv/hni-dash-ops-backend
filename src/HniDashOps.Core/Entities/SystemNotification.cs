using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities
{
    /// <summary>
    /// Thông báo hệ thống
    /// </summary>
    [Table("SYSTEM_NOTIFICATIONS")]
    public partial class SystemNotification : BaseEntity
    {
        [Key]
        [Column("ID", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("TITLE", Order = 1)]
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 200, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(200, ErrorMessage = "{0} quá dài")]
        public string Title { get; set; } = string.Empty;

        [Column("MESSAGE", Order = 2)]
        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 2000, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(2000, ErrorMessage = "{0} quá dài")]
        public string Message { get; set; } = string.Empty;

        [Column("TYPE", Order = 3)]
        [Display(Name = "Loại thông báo")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} quá dài")]
        [MaxLength(50, ErrorMessage = "{0} quá dài")]
        public string Type { get; set; } = string.Empty;

        [Column("STATUS", Order = 4)]
        [Display(Name = "Trạng thái")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} quá dài")]
        [MaxLength(20, ErrorMessage = "{0} quá dài")]
        public string Status { get; set; } = string.Empty;

        [Column("START_AT", Order = 5)]
        [Display(Name = "Thời gian bắt đầu")]
        public DateTime? StartAt { get; set; }

        [Column("END_AT", Order = 6)]
        [Display(Name = "Thời gian kết thúc")]
        public DateTime? EndAt { get; set; }

        [Column("PRIORITY", Order = 7)]
        [Display(Name = "Độ ưu tiên")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} quá dài")]
        [MaxLength(20, ErrorMessage = "{0} quá dài")]
        public string? Priority { get; set; }

        [Column("TARGET_AUDIENCE", Order = 8)]
        [Display(Name = "Đối tượng")]
        [StringLength(maximumLength: 100, ErrorMessage = "{0} quá dài")]
        [MaxLength(100, ErrorMessage = "{0} quá dài")]
        public string? TargetAudience { get; set; }

        [Column("ACTION_URL", Order = 9)]
        [Display(Name = "URL hành động")]
        [StringLength(maximumLength: 500, ErrorMessage = "{0} quá dài")]
        [MaxLength(500, ErrorMessage = "{0} quá dài")]
        public string? ActionUrl { get; set; }

        [Column("ACTION_TEXT", Order = 10)]
        [Display(Name = "Văn bản hành động")]
        [StringLength(maximumLength: 100, ErrorMessage = "{0} quá dài")]
        [MaxLength(100, ErrorMessage = "{0} quá dài")]
        public string? ActionText { get; set; }

        [Column("IS_READ", Order = 11)]
        [Display(Name = "Đã đọc")]
        public bool IsRead { get; set; } = false;

        [Column("READ_AT", Order = 12)]
        [Display(Name = "Thời gian đọc")]
        public DateTime? ReadAt { get; set; }

        [Column("READ_BY", Order = 13)]
        [Display(Name = "Người đọc")]
        public int? ReadBy { get; set; }

        [Column("METADATA", Order = 14)]
        [Display(Name = "Metadata")]
        [StringLength(maximumLength: 2000, ErrorMessage = "{0} quá dài")]
        [MaxLength(2000, ErrorMessage = "{0} quá dài")]
        public string? Metadata { get; set; }
    }
}
