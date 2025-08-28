using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("WorkerDocuments")]
    public class WorkerDocument
    {
        [Key]
        public int DocumentId { get; set; }

        public int WorkerId { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } // Resume, Certificate, Portfolio, Other

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; }

        [Required]
        [StringLength(500)]
        public string DocumentUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }
    }
}









