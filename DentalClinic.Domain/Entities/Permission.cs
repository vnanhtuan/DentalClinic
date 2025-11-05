namespace DentalClinic.Domain.Entities
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
