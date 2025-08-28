using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Workers")]
    public class Worker
    {
        [Key]
        public int WorkerId { get; set; }

        public Guid? UserId { get; set; } // Foreign key to AppUser

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(200)]
        public string? AvatarUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? CurrentPosition { get; set; }

        [StringLength(200)]
        public string? CurrentCompany { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ExpectedSalary { get; set; }

        [StringLength(50)]
        public string? WorkExperience { get; set; }

        public int? DistrictId { get; set; }
        public int? CommuneId { get; set; }
        public int? EducationLevelId { get; set; }
        public int? CareerId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District? District { get; set; }

        [ForeignKey("CommuneId")]
        public virtual Commune? Commune { get; set; }

        [ForeignKey("EducationLevelId")]
        public virtual EducationLevel? EducationLevel { get; set; }

        [ForeignKey("CareerId")]
        public virtual Career? Career { get; set; }

        public virtual ICollection<WorkerSkill> Skills { get; set; } = new HashSet<WorkerSkill>();
        public virtual ICollection<WorkerExperience> Experiences { get; set; } = new HashSet<WorkerExperience>();
        public virtual ICollection<WorkerEducation> Educations { get; set; } = new HashSet<WorkerEducation>();
        public virtual ICollection<WorkerDocument> Documents { get; set; } = new HashSet<WorkerDocument>();
        public virtual ICollection<CV> CVs { get; set; } = new HashSet<CV>();
        public virtual ICollection<JobApplication> JobApplications { get; set; } = new HashSet<JobApplication>();
        public virtual ICollection<SavedJob> SavedJobs { get; set; } = new HashSet<SavedJob>();
    }
}
