using DentalClinic.Application.DTOs.Branches;
using DentalClinic.Application.DTOs.Roles;
using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Application.DTOs.Staffs
{
    public class StaffCreateDto
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

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public List<RoleDto> Roles { get; set; } = [];
        public List<BranchDto> Branches { get; set; } = [];
    }
}
