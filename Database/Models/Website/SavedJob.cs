using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("SavedJobs")]
    public class SavedJob
    {
        [Key]
        public int SavedJobId { get; set; }

        public Guid? UserId { get; set; } // Foreign key to AppUser

        public int WorkerId { get; set; }
        public int JobId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime SavedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }

        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }

        [ForeignKey("JobId")]
        public virtual JobPosting JobPosting { get; set; }
    }
}
