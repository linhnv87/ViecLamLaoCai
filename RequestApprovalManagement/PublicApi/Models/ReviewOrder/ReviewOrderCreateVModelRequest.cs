namespace PublicApi.Models.ReviewOrder;

public class ReviewOrderCreateVModelRequest
{
    public string Name { get; set; }
    public int PrevId { get; set; }
    public int NextId { get; set; }
    public string DefaultUserId { get; set; }
    public bool IsSign { get; set; }
    public string Description { get; set; }
}
