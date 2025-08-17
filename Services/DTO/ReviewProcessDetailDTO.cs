using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ReviewProcessDetailDTO
    {
        public long ReviewProcessId { get; set; }
        public int ReviewOrderId { get; set; }
        public int? ResultLinkDocumentId { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
