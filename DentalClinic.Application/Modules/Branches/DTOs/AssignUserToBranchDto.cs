namespace DentalClinic.Application.Modules.Branches.DTOs
{
    public class AssignUserToBranchDto
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int RoleId { get; set; }
    }
}
