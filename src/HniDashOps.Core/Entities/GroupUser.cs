using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Bảng liên kết giữa Group và User (Group 1 -> n Users)
/// </summary>
[Table("GROUP_USERS")]
public partial class GroupUser : BaseEntity
{
    [Column("USER_ID", Order = 0)]
    [Display(Name = "ID Người dùng")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int UserId { get; set; }

    [Column("GROUP_ID", Order = 1)]
    [Display(Name = "ID Nhóm")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int GroupId { get; set; }

    [Column("ASSIGNED_AT", Order = 2)]
    [Display(Name = "Ngày gán")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;
}
