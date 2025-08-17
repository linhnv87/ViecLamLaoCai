using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    [Table("CfgWorkFlow")]
    public partial class CfgWorkFlow
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? PrevWorkflowId { get; set; }
        public int? NextWorkflowId { get; set; }
        public string? DefaultUserId { get; set; }
        public bool? IsSign { get; set; }
        public string? Description { get; set; }
        public int? StatusId { get; set; }
        public string? UserId { get; set; }
        public string? State { get; set; }
    }
}
