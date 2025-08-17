using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public partial class AspnetUserInRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
