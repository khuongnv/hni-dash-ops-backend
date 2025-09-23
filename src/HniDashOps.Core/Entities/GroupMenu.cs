using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Bảng liên kết giữa Group và Menu (Many-to-Many)
/// </summary>
[Table("GROUP_MENUS")]
public partial class GroupMenu : BaseEntity
{
    [Column("GROUP_ID", Order = 0)]
    [Display(Name = "ID Nhóm")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int GroupId { get; set; }

    [Column("MENU_ID", Order = 1)]
    [Display(Name = "ID Menu")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int MenuId { get; set; }

    [Column("ASSIGNED_AT", Order = 2)]
    [Display(Name = "Ngày gán")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    [ForeignKey("MenuId")]
    public virtual Menu Menu { get; set; } = null!;
}

