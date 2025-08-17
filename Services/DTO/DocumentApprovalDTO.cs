using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.AspNetCore.Http;

namespace Services.DTO
{
    public class DocumentReviewDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? DocId { get; set; }
        public int? ReviewResult { get; set; }
        public string? Comment { get; set; }
        public bool Viewed { get; set; } = false;
        public int? SubmitCount { get; set; } = 0;
        public string? FilePath { get; set; }
        public IFormFile[]? Attachment { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public int? DocumentHistoryId { get; set; }
        public bool? IsAssigned { get; set; }
        public bool? IsHighLevelLeader { get; set; }
    }
    public class DocumentApprovalGroupedDTO
    {
        public int? SubmitCount { get; set; } 
        public List<DocumentApprovalDetailDTO> Approvers { get; set; }
        public int TotalApprovers { get; set; }
        public int ReviewedCount { get; set; }
       
    }
    public class DocumentApprovalDetailDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Decision { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; }
        public bool IsReviewed => Decision == "Không đồng ý" || Decision == "Đồng ý" || Decision == "Ý kiến khác";
         public List<DocumentFileDTO> Files { get; set; }
    }

    public class DocumentApprovalSummaryDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public string Field { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Submitter { get; set; }
        public DateTime DeadlineAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<string> Approvals { get; set; }
        public List<string> Declines { get; set; }
        public List<string> NoResponses { get; set; }

    }

    public class DocumentApprovalSummaryDTO_V2
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public string Author { get; set; }
        public int Status { get; set; }
        public int FieldId { get; set; }
        public string Field { get; set; }
        public int? SubmitCount { get; set; }
        public bool IsPassed { get; set; }
        public DateTime SubmittedAt { get; set; }
        //public string Submitter { get; set; }
        public DateTime DeadlineAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<string> Approvals { get; set; }
        public List<string> Declines { get; set; }
        public List<string> NoResponses { get; set; }

    }

    public class DocumentReadFileDTO
    {
        public int DocId { get; set; }
        public Guid UserId { get; set; }
    }

}
