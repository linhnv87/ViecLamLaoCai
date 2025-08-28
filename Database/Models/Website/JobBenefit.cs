using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("JobBenefits")]
    public class JobBenefit
    {
        [Key]
        public int BenefitId { get; set; }

        public int JobId { get; set; }

        [Required]
        [StringLength(500)]
        public string BenefitDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // Salary, Health, Insurance, Training, etc.

        public int OrderNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual JobPosting JobPosting { get; set; }
    }
}









