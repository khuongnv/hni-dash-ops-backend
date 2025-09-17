using System.ComponentModel.DataAnnotations;

namespace HniDashOps.API.DTOs
{
    public class RoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PermissionResponse> Permissions { get; set; } = new();
    }

    public class CreateRoleRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AssignPermissionsRequest
    {
        [Required]
        public List<int> PermissionIds { get; set; } = new();
    }
}
