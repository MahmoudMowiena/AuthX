using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthX.Domain.Models
{
    public class User : IdentityUser
    {
        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();

        // public virtual ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    }
}


// id, email, password
