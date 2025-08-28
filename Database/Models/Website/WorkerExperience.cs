using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("WorkerExperience")]
    public class WorkerExperience
    {
        [Key]
        public int ExperienceId { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(200)]
        public string JobTitle { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public bool IsCurrentJob { get; set; } = false;

        [StringLength(2000)]
        public string? JobDescription { get; set; }

        [StringLength(1000)]
        public string? Achievements { get; set; }

        public int WorkerId { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }
    }
}









