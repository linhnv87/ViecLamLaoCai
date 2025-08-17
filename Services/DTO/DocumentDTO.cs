using Database.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.DTO
{
    public class DocumentDTO
    {
        public DocumentDTO()
        {   
        }
        public int Id { get; set; }
        public int? PriorityNumber { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public int? SubmitCount { get; set; } = 0;
        public int? FieldId { get; set; }
        public string? FieldName { get; set; }
        public int? TypeId { get; set; }
        public string? TypeName { get; set; }
        public Guid? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public DateTime? RemindDatetime { get; set; }
        public int? CurrentDocumentHistoricalId { get; set; }
        public List<string>? AssignedRoles { get; set; } = new List<string>(); 
        public int? Finalizer { get; set; } = 0;
        public string? FinalizerName { get; set; }
        public DateTime? DateEndApproval { get; set; }
        public string? StatusCode { get; set; }
        public string? PreviousStatusCode { get; set; }
        public bool? IsRetrieved { get; set; }
        public bool? HasComment { get; set; } = false;
        //public bool IsFinalized { get; set; }
        public DateTime? Modified { get; set; }
        public Guid? ModifiedBy { get; set; }        
        public DateTime? Created { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? AuthorName { get; set; }
        public bool? Deleted { get; set; }
        public List<Guid>? Users { get; set; }
        public List<Guid>? UsersSMS { get; set; }
        public List<UserInfoDTO>? AssignedUsers { get; set; }
        public List<UserInfoDTO>? SMSUsers { get; set; }

        public List<DocumentFileDTO>? DocumentFiles { get; set; }
        public List<CommentDTO>? Comments { get; set; }
        public List<DocumentReviewDTO>? Reviews { get; set; }
        public IFormFile[]? MainFiles { get; set; }
        public IFormFile[]? SideFiles { get; set; }
        public string? ReviewStatus { get; set; }
        public bool? IsTransferredSign { get; set; }
        public bool? IsReturned { get; set; }
        public bool? IsRevokeEnabled { get; set; }

        public bool? IsHistoryTabActive { get; set; }



    }
    public class GetFileDTO
    {
        public string FilePath { get; set; }
    }
    public class DocumentGroup
    {
        public string Label { get; set; }
        public int Total { get; set; }
        public List<CountDocumentByStatus> Records { get; set; }
    }
    public class CountDocumentByStatus
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Data { get; set; }
    }
}
