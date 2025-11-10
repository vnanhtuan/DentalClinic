namespace DentalClinic.Domain.Entities
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
        public string? BranchCode { get; set; } // Unique identifier code (e.g., "HQ", "BR01", "BR02")
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? District { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsMainBranch { get; set; } = false; // Flag for headquarters/main branch
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<UserBranchMapping>? UserBranches { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<Inventory>? Inventory { get; set; }
    }
}
