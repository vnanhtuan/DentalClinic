namespace DentalClinic.Application.DTOs
{
    public class RegisterStaffDto
    {
        // User info
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        // StaffDetail info
        public string? RoleTitle { get; set; } // "Bác sĩ", "Lễ tân"
        public string? Department { get; set; }
    }
}
