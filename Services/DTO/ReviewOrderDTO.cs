using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class CfgWorkFlowVModelDTO
    {
        [Required]
        public required string Name { get; set; }
        public int PrevWorkflowId { get; set; }
        public int NextWorkflowId { get; set; }
        public string? DefaultUserId { get; set; }
        public bool IsSign { get; set; }
        public string? Description { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public Guid? UserId { get; set; }
    }

    public class CfgRolesModelDTO
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }

    public class CfgUsersWorkFlowVModelDTO
    {
        public Guid UserId { get; set; }
        public string? UserFullName { get; set; }
        public bool? IsDefault { get; set; }
        public List<CfgRolesModelDTO>? Roles { get; set; }
    }

    public class ReviewOrderByIdDTO : CfgWorkFlowVModelDTO
    {
        public int? Id { get; set; }
        public List<CfgUsersWorkFlowVModelDTO>? UsersDto { get; set; }
    }
}
