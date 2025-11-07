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

        [Required(ErrorMessage = "Role is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid")]
        public int RoleId { get; set; }
    }
}
