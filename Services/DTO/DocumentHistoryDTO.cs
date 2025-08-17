using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class DocumentRetrievalRequest
    {
        public int DocumentId { get; set; }
        public string Note { get; set; }
        public string Comment { get; set; }
        public Guid CurrentUserId { get; set; }
    }
    public class DocumentHistoryDTO
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string? DocumentTitle { get; set; }
        public string? Note { get; set; }
        public int DocumentStatus { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public string? Comment { get; set; }
        public int SubmitCount { get; set; }
        public string? NameUser { get; set; }
    }
}
