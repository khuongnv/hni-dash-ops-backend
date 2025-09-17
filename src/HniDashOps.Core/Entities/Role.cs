using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Vai trò người dùng
/// </summary>
[Table("ROLES")]
public partial class Role : BaseEntity
{
    [Key]
    [Column("ID", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("NAME", Order = 1)]
    [Display(Name = "Tên vai trò")]
    [Required(ErrorMessage = "{0} không được để trống")]
    [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
    [MaxLength(50, ErrorMessage = "{0} quá dài")]
    public string Name { get; set; } = string.Empty;

    [Column("DESCRIPTION", Order = 2)]
    [Display(Name = "Mô tả")]
    [StringLength(maximumLength: 255, ErrorMessage = "{0} quá dài")]
    [MaxLength(255, ErrorMessage = "{0} quá dài")]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
