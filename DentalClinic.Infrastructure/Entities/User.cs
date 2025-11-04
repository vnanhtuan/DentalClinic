namespace DentalClinic.Infrastructure.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string UserType { get; set; } = null!; // "Staff" or "Patient"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public StaffDetail? StaffDetail { get; set; }
        public PatientDetail? PatientDetail { get; set; }

        public ICollection<UserRoleMapping>? UserRoles { get; set; }
        public ICollection<Appointment>? PatientAppointments { get; set; }
        public ICollection<Appointment>? StaffAppointments { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
