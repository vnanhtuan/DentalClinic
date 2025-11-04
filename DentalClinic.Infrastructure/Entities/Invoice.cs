namespace DentalClinic.Infrastructure.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Unpaid";

        public User Patient { get; set; } = null!;
        public ICollection<InvoiceItem>? Items { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
