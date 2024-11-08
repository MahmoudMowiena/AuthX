using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthX.Domain.Models;

public class Role: IdentityRole
{
    public virtual ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
}
