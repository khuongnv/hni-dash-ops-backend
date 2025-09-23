using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities
{
    /// <summary>
    /// Danh mục phân loại
    /// </summary>
    [Table("CATEGORIES")]
    public partial class Category : BaseEntity
    {
        [Key]
        [Column("ID", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("CODE", Order = 1)]
        [Display(Name = "Mã danh mục")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(50, ErrorMessage = "{0} quá dài")]
        public string Code { get; set; } = string.Empty;

        [Column("NAME", Order = 2)]
        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 200, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(200, ErrorMessage = "{0} quá dài")]
        public string Name { get; set; } = string.Empty;

        [Column("PARENT_ID", Order = 3)]
        [Display(Name = "Danh mục cha")]
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

        [Column("ORDER", Order = 7)]
        [Display(Name = "Thứ tự")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} phải lớn hơn hoặc bằng 0")]
        public int Order { get; set; }

        [Column("COLOR", Order = 8)]
        [Display(Name = "Màu sắc")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} quá dài")]
        [MaxLength(20, ErrorMessage = "{0} quá dài")]
        public string? Color { get; set; }

        [Column("ICON", Order = 9)]
        [Display(Name = "Icon")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} quá dài")]
        [MaxLength(50, ErrorMessage = "{0} quá dài")]
        public string? Icon { get; set; }

        [Column("IS_VISIBLE", Order = 10)]
        [Display(Name = "Hiển thị")]
        public bool IsVisible { get; set; } = true;

        [Column("TYPE", Order = 11)]
        [Display(Name = "Loại")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} quá dài")]
        [MaxLength(50, ErrorMessage = "{0} quá dài")]
        public string? Type { get; set; }

        [Column("METADATA", Order = 12)]
        [Display(Name = "Metadata")]
        [StringLength(maximumLength: 2000, ErrorMessage = "{0} quá dài")]
        [MaxLength(2000, ErrorMessage = "{0} quá dài")]
        public string? Metadata { get; set; }

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Category? Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; } = new List<Category>();
    }
}
