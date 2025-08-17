using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class WorkFlowDTO
    {
    }

    public class WorkFlowRequestDTO
    {
        public required int StatusId { get; set; }
        public required string UserId { get; set; }
        public required string State { get; set; }
    }
    public class UpdateWorkflowRequestDTO
    {
        public string? Name { get; set; }
        public int? StatusId { get; set; }
        public string? UserId { get; set; }
        public string? State { get; set; }
        public List<CfgWorkFlowUserDTO> usersDto { get; set; } = new();
    }
    public class CfgWorkFlowUserDTO
    {
        public string UserId { get; set; } = string.Empty;
    }


}
