using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Districts")]
    public class District
    {
        [Key]
        public int DistrictId { get; set; }

        [Required]
        [StringLength(100)]
        public string DistrictName { get; set; }

        [StringLength(20)]
        public string? DistrictCode { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Commune> Communes { get; set; } = new HashSet<Commune>();
        public virtual ICollection<Worker> Workers { get; set; } = new HashSet<Worker>();
        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new HashSet<JobPosting>();
    }
}









