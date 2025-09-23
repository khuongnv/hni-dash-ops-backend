using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HniDashOps.Core.Entities
{
    /// <summary>
    /// Danh mục menu hệ thống
    /// </summary>
    [Table("MENUS")]
    public partial class Menu : BaseEntity
    {
        [Key]
        [Column("ID", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("NAME", Order = 1)]
        [Display(Name = "Tên menu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "{0} cần có độ dài từ {2} đến {1} ký tự")]
        [MaxLength(100, ErrorMessage = "{0} quá dài")]
        public string Name { get; set; } = string.Empty;

        [Column("HREF", Order = 2)]
        [Display(Name = "Đường dẫn")]
        [StringLength(maximumLength: 200, ErrorMessage = "{0} quá dài")]
        [MaxLength(200, ErrorMessage = "{0} quá dài")]
        public string? Href { get; set; }

        [Column("ICON", Order = 3)]
        [Display(Name = "Icon")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} quá dài")]
        [MaxLength(50, ErrorMessage = "{0} quá dài")]
        public string? Icon { get; set; }

        [Column("ORDER", Order = 4)]
        [Display(Name = "Thứ tự")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} phải lớn hơn hoặc bằng 0")]
        public int Order { get; set; }

        [Column("PARENT_ID", Order = 5)]
        [Display(Name = "Menu cha")]
        public int? ParentId { get; set; }

        [Column("LEVEL", Order = 6)]
        [Display(Name = "Cấp độ")]
        [Range(0, 10, ErrorMessage = "{0} phải nằm trong khoảng từ {1} đến {2}")]
        public int Level { get; set; }

        [Column("DESCRIPTION", Order = 7)]
        [Display(Name = "Mô tả")]
        [StringLength(maximumLength: 500, ErrorMessage = "{0} quá dài")]
        [MaxLength(500, ErrorMessage = "{0} quá dài")]
        public string? Description { get; set; }

        [Column("IS_VISIBLE", Order = 8)]
        [Display(Name = "Hiển thị")]
        public bool IsVisible { get; set; } = true;

        [Column("TARGET", Order = 9)]
        [Display(Name = "Target")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} quá dài")]
        [MaxLength(20, ErrorMessage = "{0} quá dài")]
        public string? Target { get; set; } = "_self";

        [Column("CSS_CLASS", Order = 10)]
        [Display(Name = "CSS Class")]
        [StringLength(maximumLength: 200, ErrorMessage = "{0} quá dài")]
        [MaxLength(200, ErrorMessage = "{0} quá dài")]
        public string? CssClass { get; set; }

        [Column("DATA_ATTRIBUTES", Order = 11)]
        [Display(Name = "Data Attributes")]
        [StringLength(maximumLength: 1000, ErrorMessage = "{0} quá dài")]
        [MaxLength(1000, ErrorMessage = "{0} quá dài")]
        public string? DataAttributes { get; set; }

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Menu? Parent { get; set; }

        public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
        
        // Navigation property cho Group-based authorization
        public virtual ICollection<GroupMenu> GroupMenus { get; set; } = new List<GroupMenu>();
    }
}
