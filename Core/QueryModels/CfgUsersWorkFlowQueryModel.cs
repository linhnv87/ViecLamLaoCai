using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.QueryModels
{
    public class CfgUsersWorkFlowQueryModel
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CfgRolesModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
