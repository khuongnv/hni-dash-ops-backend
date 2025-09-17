using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Base entity với các trường chung
/// </summary>
public abstract class BaseEntity
{
    [Column("ISACTIVE", Order = 100)]
    [Display(Name = "Trạng thái hoạt động")]
    public bool IsActive { get; set; } = true;

    [Column("CREATEDAT", Order = 101)]
    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UPDATEDAT", Order = 102)]
    [Display(Name = "Ngày cập nhật")]
    public DateTime? UpdatedAt { get; set; }

    [Column("ISDELETED", Order = 103)]
    [Display(Name = "Đã xóa")]
    public bool IsDeleted { get; set; } = false;
}
