namespace PublicApi.Models.ReviewUserDetail;

public class ReviewUserDetailCreateVModelRequest
{
    public int ReviewProcessDetailId { get; set; }
    public string UserId { get; set; }
    public int ProcessStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? SignAt { get; set; }
    public string SignedLinkDocument { get; set; }
    public int? ResultLinkDocumentId { get; set; }
    public string Comments { get; set; }
}
