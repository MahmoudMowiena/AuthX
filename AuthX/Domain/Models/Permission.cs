using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthX.Domain.Models;

[Table("Permissions")]
public class Permission
{
    [Key]
    public int PermissionID { get; set; }

    [Required]
    public string PermissionName { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
}