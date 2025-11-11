namespace DentalClinic.Application.Modules.Branches.DTOs
{
    public class UpdateBranchDto
    {
        public string BranchName { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? District { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsMainBranch { get; set; }
    }
}
