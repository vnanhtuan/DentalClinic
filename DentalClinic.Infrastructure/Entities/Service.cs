namespace DentalClinic.Infrastructure.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<InvoiceItem>? InvoiceItems { get; set; }
    }
}
