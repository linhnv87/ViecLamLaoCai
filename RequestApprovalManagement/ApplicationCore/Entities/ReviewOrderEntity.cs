using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

[Table("ReviewOrder")]
public class ReviewOrderEntity : IAggregateRoot
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int PrevId { get; set; }
    public int NextId { get; set; }
    public string DefaultUserId { get; set; }
    public bool IsSign { get; set; }
    public string Description { get; set; }

    ICollection<ReviewOrderGroupDetailEntity> ReviewOrderGroupDetails { get; set; }
    ICollection<ReviewOrderUserDetailEntity> ReviewOrderUserDetails { get; set; }
}
