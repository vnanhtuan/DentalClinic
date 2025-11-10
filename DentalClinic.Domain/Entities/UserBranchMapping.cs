namespace DentalClinic.Domain.Entities
{
    /// <summary>
    /// Maps users to branches with their specific role at each branch.
    /// A user can work at multiple branches with different roles.
    /// </summary>
    public class UserBranchMapping
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int RoleId { get; set; }
        
        public bool IsActive { get; set; } = true; // Can deactivate user at specific branch
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RemovedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
        public UserRole Role { get; set; } = null!;
    }
}
