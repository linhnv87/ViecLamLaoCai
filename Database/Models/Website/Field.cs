using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Fields")]
    public class Field
    {
        [Key]
        public int FieldId { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; }

        [StringLength(20)]
        public string? FieldCode { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<CareerGroup> CareerGroups { get; set; } = new HashSet<CareerGroup>();
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new HashSet<JobPosting>();
    }
}









