namespace ApplicationCore.Entities;

public partial class TblDocumentApproval
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int? DocId { get; set; }
    public int? StatusCode { get; set; }
    public string? Comment { get; set; }
    public bool Viewed { get; set; } = false;
    public int? SubmitCount { get; set; }
    public string? FilePath { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? Modified { get; set; }
    public bool? Deleted { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? Created { get; set; }        
}
