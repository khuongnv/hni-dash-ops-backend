using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Nhóm người dùng trong hệ thống
/// </summary>
[Table("GROUPS")]
public partial class Group : BaseEntity
{
    [Key]
    [Column("ID", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("NAME", Order = 1)]
    [Display(Name = "Tên nhóm")]
    [Required(ErrorMessage = "{0} không được để trống")]
    [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
    [MaxLength(100, ErrorMessage = "{0} quá dài")]
    public string Name { get; set; } = string.Empty;

    [Column("DESCRIPTION", Order = 2)]
    [Display(Name = "Mô tả")]
    [StringLength(maximumLength: 500, ErrorMessage = "{0} quá dài")]
    [MaxLength(500, ErrorMessage = "{0} quá dài")]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
    public virtual ICollection<GroupMenu> GroupMenus { get; set; } = new List<GroupMenu>();
    
    // Future-ready properties (comment out for now)
    // public virtual ICollection<GroupDepartment> GroupDepartments { get; set; } = new List<GroupDepartment>();
    // public virtual ICollection<GroupCategory> GroupCategories { get; set; } = new List<GroupCategory>();
}

