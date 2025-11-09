using DentalClinic.Application.DTOs.Roles;

namespace DentalClinic.Application.DTOs.Staffs
{
    public class StaffDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleColor { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }
}
