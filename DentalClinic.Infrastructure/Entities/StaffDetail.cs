namespace DentalClinic.Infrastructure.Entities
{
    public class StaffDetail
    {
        public int StaffDetailId { get; set; }
        public int UserId { get; set; }
        public string? RoleTitle { get; set; }
        public string? Department { get; set; }

        public User User { get; set; } = null!;
    }
}
