using DentalClinic.Application.Modules.Branches.DTOs;
using DentalClinic.Application.Modules.Roles.DTOs;

namespace DentalClinic.Application.Modules.Staffs.DTOs
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
        public bool IsActive { get; set; }
        public bool HasActiveAssignments { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<RoleDto> Roles { get; set; } = [];
        public List<BranchDto> Branches { get; set; } = [];
    }
}
