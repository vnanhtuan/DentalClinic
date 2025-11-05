namespace DentalClinic.Domain.Entities
{
    public class PatientDetail
    {
        public int PatientDetailId { get; set; }
        public int UserId { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? MedicalNote { get; set; }

        public User User { get; set; } = null!;
    }
}
