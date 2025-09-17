using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.DTOs
{
    public class DepartmentResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? MapId { get; set; }
        public int Level { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<DepartmentResponse> Children { get; set; } = new();
        public int UserCount { get; set; }
    }

    public class CreateDepartmentRequest
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
    }

    public class UpdateDepartmentRequest
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
        public bool IsActive { get; set; } = true;
    }

    public class AssignUserToDepartmentRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
