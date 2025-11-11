using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Application.Modules.Roles.DTOs
{
    public class RoleCreateUpdateDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Color { get; set; }
        // (The future, add List<int> PermissionIds in here)
    }
}
