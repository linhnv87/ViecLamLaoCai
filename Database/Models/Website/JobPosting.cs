using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("JobPostings")]
    public class JobPosting
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        [StringLength(200)]
        public string JobTitle { get; set; }

        [StringLength(1000)]
        public string? JobDescription { get; set; }

        [StringLength(500)]
        public string? Requirements { get; set; }

        [StringLength(500)]
        public string? Benefits { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinSalary { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxSalary { get; set; }

        [StringLength(50)]
        public string? SalaryType { get; set; } // Monthly, Yearly, Hourly

        [StringLength(100)]
        public string? WorkLocation { get; set; }

        [StringLength(50)]
        public string? EmploymentType { get; set; } // Full-time, Part-time, Contract, Internship

        [StringLength(100)]
        public string? WorkingDays { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ApplicationDeadline { get; set; }

        public int CompanyId { get; set; }
        public int? FieldId { get; set; }
        public int? CareerId { get; set; }
        public int? DistrictId { get; set; }

        public bool IsFeatured { get; set; } = false;
        public bool IsUrgent { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Active";
        public int Views { get; set; } = 0;
        public int ApplicationsCount { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }

        [ForeignKey("CareerId")]
        public virtual Career? Career { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District? District { get; set; }

        public virtual ICollection<JobRequirement> JobRequirements { get; set; } = new HashSet<JobRequirement>();
        public virtual ICollection<JobBenefit> JobBenefits { get; set; } = new HashSet<JobBenefit>();
        public virtual ICollection<JobApplication> JobApplications { get; set; } = new HashSet<JobApplication>();
        public virtual ICollection<SavedJob> SavedJobs { get; set; } = new HashSet<SavedJob>();
    }
}









