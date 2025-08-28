using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("InterviewResults")]
    public class InterviewResult
    {
        [Key]
        public int ResultId { get; set; }

        public int InterviewId { get; set; }

        public int? Score { get; set; } // 1-10 or 1-100

        [StringLength(2000)]
        public string? Feedback { get; set; }

        [Required]
        [StringLength(20)]
        public string Decision { get; set; } // Passed, Failed, On Hold, Need Second Interview

        [StringLength(500)]
        public string? NextStep { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("InterviewId")]
        public virtual Interview Interview { get; set; }
    }
}









