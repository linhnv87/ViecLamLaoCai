using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.QueryModels
{
    public class DocumentSummaryQueryModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public int? FieldId { get; set; }
        public string? AuthorId { get; set; }
        public DateTime? SubmitFrom { get; set; }
        public DateTime? SubmitTo { get; set; }
        public bool? IsPassed { get; set; }
    }
}
