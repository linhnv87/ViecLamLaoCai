using ApplicationCore.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

[Table("ReviewUserDetail", Schema = "dbo")]
public class ReviewUserDetailEntity : IAggregateRoot
{
    [Key]
    public int Id { get; set; }

    public int ReviewProcessDetailId { get; set; }

    [Required]
    [StringLength(450)]
    public string UserId { get; set; }

    public int ProcessStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? SignAt { get; set; }

    public string SignedLinkDocument { get; set; }

    public int? ResultLinkDocumentId { get; set; }

    public string Comments { get; set; }

    public virtual ReviewProcessDetailEntity ReviewProcessDetail { get; set; }
}
