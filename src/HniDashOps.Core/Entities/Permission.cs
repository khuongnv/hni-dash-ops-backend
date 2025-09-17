using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Quyền hạn hệ thống
/// </summary>
[Table("PERMISSIONS")]
public partial class Permission : BaseEntity
{
    [Key]
    [Column("ID", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("NAME", Order = 1)]
    [Display(Name = "Tên quyền")]
    [Required(ErrorMessage = "{0} không được để trống")]
    [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
    [MaxLength(100, ErrorMessage = "{0} quá dài")]
    public string Name { get; set; } = string.Empty;

    [Column("DESCRIPTION", Order = 2)]
    [Display(Name = "Mô tả")]
    [StringLength(maximumLength: 255, ErrorMessage = "{0} quá dài")]
    [MaxLength(255, ErrorMessage = "{0} quá dài")]
    public string? Description { get; set; }

    [Column("RESOURCE", Order = 3)]
    [Display(Name = "Tài nguyên")]
    [StringLength(maximumLength: 100, ErrorMessage = "{0} quá dài")]
    [MaxLength(100, ErrorMessage = "{0} quá dài")]
    public string? Resource { get; set; }

    [Column("ACTION", Order = 4)]
    [Display(Name = "Hành động")]
    [StringLength(maximumLength: 50, ErrorMessage = "{0} quá dài")]
    [MaxLength(50, ErrorMessage = "{0} quá dài")]
    public string? Action { get; set; }

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
