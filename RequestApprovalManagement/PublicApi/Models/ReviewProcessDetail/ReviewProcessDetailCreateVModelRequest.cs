namespace PublicApi.Models.ReviewProcessDetail;

public class ReviewProcessDetailCreateVModelRequest
{
    public int CurrentProcessId { get; set; }
    public int ReviewProcessId { get; set; }
    public int ProcessStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? ResultLinkDocumentId { get; set; }
    public DateTime? Deadline { get; set; }
}
