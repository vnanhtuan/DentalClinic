namespace DentalClinic.Infrastructure.Entities
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
