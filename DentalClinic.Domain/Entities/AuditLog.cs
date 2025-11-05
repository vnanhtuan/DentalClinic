using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int RecordId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
