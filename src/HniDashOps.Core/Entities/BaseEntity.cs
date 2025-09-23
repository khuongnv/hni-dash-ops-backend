using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Base entity với các trường chung
/// </summary>
public abstract class BaseEntity
{
    [Column("IS_ACTIVE", Order = 100)]
    [Display(Name = "Trạng thái hoạt động")]
    public bool IsActive { get; set; } = true;

    [Column("CREATED_AT", Order = 101)]
    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("CREATED_BY", Order = 102)]
    [Display(Name = "Người tạo")]
    public int? CreatedBy { get; set; }

    [Column("UPDATED_AT", Order = 103)]
    [Display(Name = "Ngày cập nhật")]
    public DateTime? UpdatedAt { get; set; }

    [Column("UPDATED_BY", Order = 104)]
    [Display(Name = "Người cập nhật")]
    public int? UpdatedBy { get; set; }

    [Column("IS_DELETED", Order = 105)]
    [Display(Name = "Đã xóa")]
    public bool IsDeleted { get; set; } = false;

    [Column("DELETED_AT", Order = 106)]
    [Display(Name = "Ngày xóa")]
    public DateTime? DeletedAt { get; set; }

    [Column("DELETED_BY", Order = 107)]
    [Display(Name = "Người xóa")]
    public int? DeletedBy { get; set; }
}
