using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthX.Domain.Models;

[Table("Sessions")]
public class SessionModel
{
    [Key]
    public int SessionID { get; set; }

    [ForeignKey(nameof(UserModel))]
    public string UserID { get; set; }

    [Required]
    public string Token { get; set; }

    public DateTime Expiration { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual UserModel UserModel { get; set; }
}