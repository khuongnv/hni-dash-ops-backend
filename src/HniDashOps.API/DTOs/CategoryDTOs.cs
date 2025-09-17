using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.DTOs
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? MapId { get; set; }
        public int Level { get; set; }
        public string? Note { get; set; }
        public int Order { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public bool IsVisible { get; set; }
        public string? Type { get; set; }
        public string? Metadata { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryResponse> Children { get; set; } = new();
        public int ChildrenCount { get; set; }
    }

    public class CreateCategoryRequest
    {
        [Required, StringLength(50, MinimumLength = 2)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }
        [StringLength(100)]
        public string? MapId { get; set; }
        public int Level { get; set; } = 0;
        [StringLength(1000)]
        public string? Note { get; set; }
        public int Order { get; set; } = 0;
        [StringLength(20)]
        public string? Color { get; set; }
        [StringLength(50)]
        public string? Icon { get; set; }
        public bool IsVisible { get; set; } = true;
        [StringLength(50)]
        public string? Type { get; set; }
        [StringLength(2000)]
        public string? Metadata { get; set; }
    }

    public class UpdateCategoryRequest
    {
        [Required, StringLength(50, MinimumLength = 2)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }
        [StringLength(100)]
        public string? MapId { get; set; }
        public int Level { get; set; } = 0;
        [StringLength(1000)]
        public string? Note { get; set; }
        public int Order { get; set; } = 0;
        [StringLength(20)]
        public string? Color { get; set; }
        [StringLength(50)]
        public string? Icon { get; set; }
        public bool IsVisible { get; set; } = true;
        [StringLength(50)]
        public string? Type { get; set; }
        [StringLength(2000)]
        public string? Metadata { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ReorderCategoriesRequest
    {
        public int? ParentId { get; set; }
        [Required]
        public List<int> CategoryIds { get; set; } = new();
    }
}
