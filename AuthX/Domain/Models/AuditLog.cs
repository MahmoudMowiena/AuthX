using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthX.Domain.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int LogID { get; set; }

        [ForeignKey(nameof(User))]
        public string UserID { get; set; }

        // [ForeignKey(nameof(Session))]
        // public int SessionID { get; set; }

        [Required]
        public string Action { get; set; }

        public DateTime Timestamp { get; set; }

        [Required]
        public string IPAddress { get; set; }

        public virtual User User { get; set; }

        // public virtual Session Session { get; set; }
    }
}
