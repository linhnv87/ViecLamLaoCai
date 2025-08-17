using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    [Table("CfgWorkFlowGroup")]
    public partial class CfgWorkFlowGroup
    {
        [Key]
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public required string RoleId { get; set; }
        public string? DefaultUserId { get; set; }
        public bool IsDefault { get; set; }

        [ForeignKey("WorkflowId")]
        public virtual required CfgWorkFlow CfgWorkFlow { get; set; }
    }
}
