namespace ApplicationCore.Entities;

public partial class TblStatus
{
    public int Id { get; set; }
    public string? StatusCode { get; set; }
    public string? Title { get; set; }
    public DateTime? Modified { get; set; }
    public bool? Deleted { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? Created { get; set; }        
}
