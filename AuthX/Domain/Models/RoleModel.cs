using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthX.Domain.Models;

public class RoleModel: IdentityRole
{
    public virtual ICollection<PermissionModel> Permissions { get; set; } = new HashSet<PermissionModel>();
    public virtual ICollection<UserModel> Users { get; set; } = new HashSet<UserModel>();
}

// id, rolename