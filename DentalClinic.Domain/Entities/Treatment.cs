namespace DentalClinic.Domain.Entities
{
    public class Treatment
    {
        public int TreatmentId { get; set; }
        public int AppointmentId { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Prescription { get; set; }
        public string? Images { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Appointment Appointment { get; set; } = null!;
    }
}
