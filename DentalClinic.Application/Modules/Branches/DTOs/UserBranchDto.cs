namespace DentalClinic.Application.Modules.Branches.DTOs
{
    public class UserBranchDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
