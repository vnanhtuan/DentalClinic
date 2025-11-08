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

        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
