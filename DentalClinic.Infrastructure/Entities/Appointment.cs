namespace DentalClinic.Infrastructure.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string? Notes { get; set; }

        public User Patient { get; set; } = null!;
        public User Staff { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public ICollection<Treatment>? Treatments { get; set; }
    }
}
