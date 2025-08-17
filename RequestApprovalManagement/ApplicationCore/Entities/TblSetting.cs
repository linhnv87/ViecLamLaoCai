namespace ApplicationCore.Entities;

public partial class TblSetting
{
    public int Id { get; set; }
    public string? TitleSetting { get; set; }
    public string? Value { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }
    public bool? Deleted { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid? CreatedBy { get; set; }
}
