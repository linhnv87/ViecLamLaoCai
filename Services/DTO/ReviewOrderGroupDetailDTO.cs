using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ReviewOrderGroupDetailVModelDTO
    {
        [Required]
        public required string RoleId { get; set; }
        public string? DefaultUserId { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ReviewOrderGroupDetailByIdlDTO : ReviewOrderGroupDetailVModelDTO
    {
        public int Id { get; set; }
    }
}
