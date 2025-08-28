using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Interviews")]
    public class Interview
    {
        [Key]
        public int InterviewId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InterviewDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan InterviewTime { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, Rescheduled

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("ApplicationId")]
        public virtual JobApplication JobApplication { get; set; }

        public virtual InterviewResult? InterviewResult { get; set; }
    }
}









