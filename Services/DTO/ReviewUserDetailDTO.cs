using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ReviewUserDetailDTO
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public int ReviewStatus { get; set; }
        [Required]
        public int DocId { get; set; }
        [Required]
        public required string Comments { get; set; }
    }
}
