using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Quyền hạn vai trò
/// </summary>
[Table("ROLEPERMISSIONS")]
public partial class RolePermission : BaseEntity
{
    [Key]
    [Column("ID", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("ROLEID", Order = 1)]
    [Display(Name = "Vai trò")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int RoleId { get; set; }

    [Column("PERMISSIONID", Order = 2)]
    [Display(Name = "Quyền hạn")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int PermissionId { get; set; }

    [Column("ASSIGNEDAT", Order = 3)]
    [Display(Name = "Ngày gán")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("PermissionId")]
    public virtual Permission Permission { get; set; } = null!;
}
