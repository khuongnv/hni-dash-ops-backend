using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities
{
    /// <summary>
    /// Phòng ban tổ chức
    /// </summary>
    [Table("DEPARTMENTS")]
    public partial class Department : BaseEntity
    {
        [Key]
        [Column("ID", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("CODE", Order = 1)]
        [Display(Name = "Mã phòng ban")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(50, ErrorMessage = "{0} quá dài")]
        public string Code { get; set; } = string.Empty;

        [Column("NAME", Order = 2)]
        [Display(Name = "Tên phòng ban")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 200, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(200, ErrorMessage = "{0} quá dài")]
        public string Name { get; set; } = string.Empty;

        [Column("PARENT_ID", Order = 3)]
        [Display(Name = "Phòng ban cha")]
        public int? ParentId { get; set; }

        [Column("MAP_ID", Order = 4)]
        [Display(Name = "Mã ánh xạ")]
        [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(100, ErrorMessage = "{0} quá dài")]
        public string? MapId { get; set; }

        [Column("LEVEL", Order = 5)]
        [Display(Name = "Cấp độ")]
        [Range(0, 10, ErrorMessage = "{0} phải nằm trong khoảng từ {1} đến {2}")]
        public int Level { get; set; }

        [Column("NOTE", Order = 6)]
        [Display(Name = "Ghi chú")]
        [StringLength(maximumLength: 1000, ErrorMessage = "{0} quá dài")]
        [MaxLength(1000, ErrorMessage = "{0} quá dài")]
        public string? Note { get; set; }

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Department? Parent { get; set; }

        public virtual ICollection<Department> Children { get; set; } = new List<Department>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
