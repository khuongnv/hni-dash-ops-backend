using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities;

/// <summary>
/// Người dùng hệ thống
/// </summary>
[Table("USERS")]
public partial class User : BaseEntity
{
    [Key]
    [Column("ID", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("USERNAME", Order = 1)]
    [Display(Name = "Tên đăng nhập")]
    [Required(ErrorMessage = "{0} không được để trống")]
    [StringLength(maximumLength: 100, MinimumLength = 3, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
    [MaxLength(100, ErrorMessage = "{0} quá dài")]
    public string Username { get; set; } = string.Empty;

    [Column("EMAIL", Order = 2)]
    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0} không được để trống")]
    [EmailAddress(ErrorMessage = "{0} không hợp lệ")]
    [StringLength(maximumLength: 255, ErrorMessage = "{0} quá dài")]
    [MaxLength(255, ErrorMessage = "{0} quá dài")]
    public string Email { get; set; } = string.Empty;

    [Column("PASSWORDHASH", Order = 3)]
    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "{0} không được để trống")]
    [StringLength(maximumLength: 255, ErrorMessage = "{0} quá dài")]
    [MaxLength(255, ErrorMessage = "{0} quá dài")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("FIRSTNAME", Order = 4)]
    [Display(Name = "Tên")]
    [StringLength(maximumLength: 100, ErrorMessage = "{0} quá dài")]
    [MaxLength(100, ErrorMessage = "{0} quá dài")]
    public string? FirstName { get; set; }

    [Column("LASTNAME", Order = 5)]
    [Display(Name = "Họ")]
    [StringLength(maximumLength: 100, ErrorMessage = "{0} quá dài")]
    [MaxLength(100, ErrorMessage = "{0} quá dài")]
    public string? LastName { get; set; }

    [Column("PHONENUMBER", Order = 6)]
    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "{0} không hợp lệ")]
    [StringLength(maximumLength: 20, ErrorMessage = "{0} quá dài")]
    [MaxLength(20, ErrorMessage = "{0} quá dài")]
    public string? PhoneNumber { get; set; }

    [Column("EMAILCONFIRMED", Order = 7)]
    [Display(Name = "Xác thực email")]
    public bool EmailConfirmed { get; set; } = false;

    [Column("LASTLOGINAT", Order = 8)]
    [Display(Name = "Lần đăng nhập cuối")]
    public DateTime? LastLoginAt { get; set; }

    [Column("DEPARTMENTID", Order = 9)]
    [Display(Name = "Phòng ban")]
    public int? DepartmentId { get; set; }

    // Navigation properties
    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
