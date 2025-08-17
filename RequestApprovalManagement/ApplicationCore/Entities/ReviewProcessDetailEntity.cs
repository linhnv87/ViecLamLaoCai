using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ReviewProcessDetail", Schema = "dbo")]
public class ReviewProcessDetailEntity : IAggregateRoot
{
    [Key]
    public int Id { get; set; }

    public int CurrentProcessId { get; set; }

    public int ReviewProcessId { get; set; }

    public int ProcessStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ResultLinkDocumentId { get; set; }

    public DateTime? Deadline { get; set; }

    public virtual ReviewProcessEntity ReviewProcess { get; set; }
    public virtual ICollection<ReviewUserDetailEntity> ReviewUserDetails { get; set; }

}
