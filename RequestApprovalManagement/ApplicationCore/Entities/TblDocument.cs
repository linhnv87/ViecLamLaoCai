namespace ApplicationCore.Entities;

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
    public int? Finalizer { get; set; } = 0;
    public DateTime? DateEndApproval { get; set; }
    public int? StatusCode { get; set; }
    public int? PreviousStatusCode { get;set; }
    public bool? IsRetrieved { get; set; }
    //public bool IsFinalized { get; set; } = false;
    public DateTime? Modified { get; set; }
    public bool? Deleted { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? Created { get; set; }              
}
