using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class TblDocument    
    {
        public TblDocument()
        {                       
        }
        public int Id { get; set; }
        public int? PriorityNumber { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public int? SubmitCount { get; set; } = 0;
        public int? FieldId { get; set; }
        public int? TypeId { get; set; }
        public Guid? AssigneeId { get; set; }
        public DateTime? RemindDatetime { get; set; }
        public int? CurrentDocumentHistoricalId { get; set; }
        public int? Finalizer { get; set; } = 0;
        public DateTime? DateEndApproval { get; set; }
        public string? StatusCode { get; set; }
        public string? PreviousStatusCode { get;set; }
        public bool? IsRetrieved { get; set; }        
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? Created { get; set; }
    }
}
