namespace ApplicationCore.Entities;

public partial class TblField
{        
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool? Active { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? Modified { get; set; }
    public DateTime? Created { get; set; }
    public bool? Deleted { get; set; }
    
}
