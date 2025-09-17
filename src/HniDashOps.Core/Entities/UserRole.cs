using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Vai trò người dùng
/// </summary>
[Table("USERROLES")]
public partial class UserRole : BaseEntity
{
    [Key]
    [Column("ID", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("USERID", Order = 1)]
    [Display(Name = "Người dùng")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int UserId { get; set; }

    [Column("ROLEID", Order = 2)]
    [Display(Name = "Vai trò")]
    [Required(ErrorMessage = "{0} không được để trống")]
    public int RoleId { get; set; }

    [Column("ASSIGNEDAT", Order = 3)]
    [Display(Name = "Ngày gán")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [Column("EXPIRESAT", Order = 4)]
    [Display(Name = "Ngày hết hạn")]
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}
