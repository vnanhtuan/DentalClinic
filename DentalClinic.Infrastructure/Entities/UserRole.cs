using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Infrastructure.Entities
{
    public class UserRole
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<UserRoleMapping>? UserMappings { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
