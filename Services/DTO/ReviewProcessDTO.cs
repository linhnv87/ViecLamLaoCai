using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ReviewProcessDTO
    {
        public long Id { get; set; }
        [Required]
        public required string CreatedBy { get; set; }
        [Required]
        public int DocumentId { get; set; }
        public string? Comments { get; set; }
        public DateTime? Deadline { get; set; }
    }

    public class ReviewProcessVModelDTO
    {
        [Required]
        public long ReviewProcessId { get; set; }
        [Required]
        public int ProcessStatusId { get; set; }
        [Required]
        public required List<Guid> UserIds { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
