using DentalClinic.Application.DTOs.Branches;
using DentalClinic.Application.DTOs.Roles;
using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Application.DTOs.Staffs
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

        public List<int> RoleIds { get; set; } = [];
        public List<int> BranchIds { get; set; } = [];

        public List<RoleDto> Roles { get; set; } = [];
        public List<BranchDto> Branches { get; set; } = [];
    }
}
