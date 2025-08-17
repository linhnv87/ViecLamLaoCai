namespace PublicApi.Models.ReviewOrderGroupDetail;

public class ReviewOrderGroupDetailCreateVModelRequest
{
    public int ReviewOrderId { get; set; }
    public string RoleId { get; set; }
    public string DefaultUserId { get; set; }
    public bool IsDefault { get; set; }
}
