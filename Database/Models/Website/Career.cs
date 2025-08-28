using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Careers")]
    public class Career
    {
        [Key]
        public int CareerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CareerName { get; set; }

        [StringLength(20)]
        public string? CareerCode { get; set; }

        public int? SpecializationId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("SpecializationId")]
        public virtual Specialization? Specialization { get; set; }

        public virtual ICollection<Worker> Workers { get; set; } = new HashSet<Worker>();
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new HashSet<JobPosting>();
    }
}








