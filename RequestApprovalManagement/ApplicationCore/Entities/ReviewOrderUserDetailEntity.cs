using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

[Table("ReviewOrderUserDetail")]
public class ReviewOrderUserDetailEntity : IAggregateRoot
{
    [Key]
    public int Id { get; set; }
    public int ReviewOrderId { get; set; }
    public string UserId { get; set; }
    public bool IsDefault { get; set; }

    [ForeignKey("ReviewOrderId")]
    public ReviewOrderEntity ReviewOrder { get; set; }
}
