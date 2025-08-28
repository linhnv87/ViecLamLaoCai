using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("JobRequirements")]
    public class JobRequirement
    {
        [Key]
        public int RequirementId { get; set; }

        public int JobId { get; set; }

        [Required]
        [StringLength(500)]
        public string RequirementDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // Required, Preferred, Nice to have

        public int OrderNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual JobPosting JobPosting { get; set; }
    }
}









