namespace DentalClinic.Application.DTOs.Branches
{
    public class BranchDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
        public string? BranchCode { get; set; }
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? District { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsMainBranch { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
