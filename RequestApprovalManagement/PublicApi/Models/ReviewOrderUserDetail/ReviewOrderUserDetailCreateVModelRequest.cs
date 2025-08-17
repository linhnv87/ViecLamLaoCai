namespace PublicApi.Models.ReviewOrderUserDetail;

public class ReviewOrderUserDetailCreateVModelRequest
{
    public int ReviewOrderId { get; set; }
    public string UserId { get; set; }
    public bool IsDefault { get; set; }
}
