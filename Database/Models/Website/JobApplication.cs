using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("JobApplications")]
    public class JobApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        public Guid? UserId { get; set; } // Foreign key to AppUser

        public int JobId { get; set; }
        public int WorkerId { get; set; }

        [StringLength(1000)]
        public string? CoverLetter { get; set; }

        [StringLength(500)]
        public string? ResumeUrl { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ExpectedSalary { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AvailableStartDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Interviewed, Accepted, Rejected

        [DataType(DataType.DateTime)]
        public DateTime? InterviewDate { get; set; }

        [StringLength(500)]
        public string? InterviewLocation { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }

        [ForeignKey("JobId")]
        public virtual JobPosting JobPosting { get; set; }

        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }

        public virtual Interview? Interview { get; set; }
    }
}
