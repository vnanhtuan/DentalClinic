using DentalClinic.Application.Modules.Branches.DTOs;
using DentalClinic.Application.Modules.Roles.DTOs;
using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Application.Modules.Staffs.DTOs
{
    public class StaffUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public bool IsActive { get; set; }

        public List<int> RoleIds { get; set; } = [];
        public List<int> BranchIds { get; set; } = [];

        public List<RoleDto> Roles { get; set; } = [];
        public List<BranchDto> Branches { get; set; } = [];
    }
}
