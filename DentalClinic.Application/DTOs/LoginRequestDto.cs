namespace DentalClinic.Application.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int? SelectedBranchId { get; set; } 
    }
}
