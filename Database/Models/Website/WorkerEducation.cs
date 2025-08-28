using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("WorkerEducation")]
    public class WorkerEducation
    {
        [Key]
        public int EducationId { get; set; }

        public int WorkerId { get; set; }
        public int EducationLevelId { get; set; }

        [Required]
        [StringLength(200)]
        public string SchoolName { get; set; }

        [StringLength(200)]
        public string? Major { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? GPA { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }

        [ForeignKey("EducationLevelId")]
        public virtual EducationLevel EducationLevel { get; set; }
    }
}









