using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

[Table("ReviewOrderGroupDetail")]
public class ReviewOrderGroupDetailEntity : IAggregateRoot
{
    public int Id { get; set; }
    public int ReviewOrderId { get; set; }
    public string RoleId { get; set; }
    public string DefaultUserId { get; set; }
    public bool IsDefault { get; set; }

    [ForeignKey("ReviewOrderId")]
    public ReviewOrderEntity ReviewOrder { get; set; }
}
