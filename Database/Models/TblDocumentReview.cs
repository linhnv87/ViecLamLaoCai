using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public partial class TblDocumentReview
    {
        public TblDocumentReview()
        {
        }
        public int Id { get; set; }
        public string? Title {  get; set; }
        public int? DocId {  get; set; }
        public int? ReviewResult {  get; set; }
        public Guid? UserId {  get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public string? Comment {  get; set; }
        public int? SubmitCount { get;set; }
        public bool? Viewed { get; set; }
        public string? FilePath { get; set; }
        public int? DocumentHistoryId { get; set; }
        public bool? IsAssigned { get; set; }
        public bool? IsHighLevelLeader { get; set; }
        public bool? IsActiveSMS { get; set; }
        public bool? IsRetrieved { get; set; }

    }
}