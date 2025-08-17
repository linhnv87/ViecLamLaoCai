using ApplicationCore.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

[Table("ReviewProcesses", Schema = "dbo")]
public class ReviewProcessEntity : IAggregateRoot
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(450)]
    public string CreatedBy { get; set; }

    public int DocumentId { get; set; }

    public DateTime ReviewDate { get; set; }

    public int DocumentStatus { get; set; }

    public string Comments { get; set; }

    public DateTime? Deadline { get; set; }

    public virtual ICollection<ReviewProcessDetailEntity> ReviewProcessDetails { get; set; }
}
