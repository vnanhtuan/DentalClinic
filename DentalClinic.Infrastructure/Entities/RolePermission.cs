namespace DentalClinic.Infrastructure.Entities
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public UserRole Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
