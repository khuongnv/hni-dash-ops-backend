using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.DTOs
{
    public class MenuResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Href { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int Level { get; set; }
        public string? Description { get; set; }
        public bool IsVisible { get; set; }
        public string? Target { get; set; }
        public string? CssClass { get; set; }
        public string? DataAttributes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MenuResponse> Children { get; set; } = new();
    }

    public class CreateMenuRequest
    {
        [Required, StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Href { get; set; }
        [StringLength(50)]
        public string? Icon { get; set; }
        public int Order { get; set; } = 0;
        public int? ParentId { get; set; }
        public int Level { get; set; } = 0;
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsVisible { get; set; } = true;
        [StringLength(20)]
        public string? Target { get; set; } = "_self";
        [StringLength(200)]
        public string? CssClass { get; set; }
        [StringLength(1000)]
        public string? DataAttributes { get; set; }
    }

    public class UpdateMenuRequest
    {
        [Required, StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Href { get; set; }
        [StringLength(50)]
        public string? Icon { get; set; }
        public int Order { get; set; } = 0;
        public int? ParentId { get; set; }
        public int Level { get; set; } = 0;
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsVisible { get; set; } = true;
        [StringLength(20)]
        public string? Target { get; set; } = "_self";
        [StringLength(200)]
        public string? CssClass { get; set; }
        [StringLength(1000)]
        public string? DataAttributes { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ReorderMenusRequest
    {
        public int? ParentId { get; set; }
        [Required]
        public List<int> MenuIds { get; set; } = new();
    }
}
