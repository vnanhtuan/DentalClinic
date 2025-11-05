namespace DentalClinic.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";
        public string? Notes { get; set; }

        public Invoice Invoice { get; set; } = null!;
    }
}
