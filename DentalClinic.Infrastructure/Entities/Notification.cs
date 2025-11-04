namespace DentalClinic.Infrastructure.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = "InApp"; // SMS, Email, InApp
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
