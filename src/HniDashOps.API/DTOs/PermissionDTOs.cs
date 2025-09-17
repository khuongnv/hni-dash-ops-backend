using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.DTOs
{
    public class PermissionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Resource { get; set; }
        public string? Action { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePermissionRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required, StringLength(50)]
        public string Resource { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Action { get; set; } = string.Empty;
    }

    public class UpdatePermissionRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required, StringLength(50)]
        public string Resource { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Action { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
