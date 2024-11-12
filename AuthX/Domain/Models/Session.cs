// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

namespace AuthX.Domain.Models;

// [Table("Sessions")]
public class Session
{
    // [Key]
    // public int SessionID { get; set; }

    // [ForeignKey(nameof(User))]
    public string UserID { get; set; }

    // [Required]
    public string Token { get; set; }

    public DateTime ExpirationDate { get; set; }

    public DateTime CreatedDate { get; set; }

    // public virtual User User { get; set; }
}