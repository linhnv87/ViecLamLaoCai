namespace PublicApi.Models.ReviewProcess;

public class ReviewProcessCreateVModelRequest
{
    public string CreatedBy { get; set; }
    public int DocumentId { get; set; }
    public DateTime ReviewDate { get; set; }
    public int DocumentStatus { get; set; }
    public string Comments { get; set; }
    public DateTime? Deadline { get; set; }
}
