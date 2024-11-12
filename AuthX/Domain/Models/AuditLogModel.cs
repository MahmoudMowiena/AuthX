using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthX.Domain.Models
{
    [Table("AuditLogs")]
    public class AuditLogModel
    {
        [Key]
        public int LogID { get; set; }

        [ForeignKey(nameof(UserModel))]
        public string UserID { get; set; }

        [ForeignKey(nameof(SessionModel))]
        public int SessionID { get; set; }

        [Required]
        public string Action { get; set; }

        public DateTime Timestamp { get; set; }

        [Required]
        public string IPAddress { get; set; }

        public virtual UserModel UserModel { get; set; }

        public virtual SessionModel SessionModel { get; set; }
    }
}
