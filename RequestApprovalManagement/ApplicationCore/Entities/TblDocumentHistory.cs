namespace ApplicationCore.Entities;

public enum EHistoryStatus
{
    Default,
    Created,
    SentToApprove,
    Retrieved,
    ReturnedToEdit,
    Approved,
    Completed,
    Returned,
    Published
}
public class TblDocumentHistory
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string? DocumentTitle { get; set; }
    public string? Note { get; set; }
    public EHistoryStatus HistoryStatus { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public Guid? CreatedBy { get; set; }
}
