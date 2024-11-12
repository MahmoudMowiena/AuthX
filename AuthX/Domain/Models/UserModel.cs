using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthX.Domain.Models
{
    public class UserModel : IdentityUser
    {
        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<RoleModel> Roles { get; set; } = new HashSet<RoleModel>();

        public virtual ICollection<SessionModel> Sessions { get; set; } = new HashSet<SessionModel>();
    }
}


// id, email, password
