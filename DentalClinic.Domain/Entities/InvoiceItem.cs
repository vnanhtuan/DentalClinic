namespace DentalClinic.Domain.Entities
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public Invoice Invoice { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
