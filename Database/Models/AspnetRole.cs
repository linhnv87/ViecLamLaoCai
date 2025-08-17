using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class AspnetRole
    {
        public AspnetRole()
        {
            Users = new HashSet<AspnetUser>();
        }

        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string LoweredRoleName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual AspnetApplication Application { get; set; } = null!;

        public virtual ICollection<AspnetUser> Users { get; set; }
    }
}
