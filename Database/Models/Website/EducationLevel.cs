using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("EducationLevels")]
    public class EducationLevel
    {
        [Key]
        public int LevelId { get; set; }

        [Required]
        [StringLength(100)]
        public string LevelName { get; set; }

        [StringLength(20)]
        public string? LevelCode { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Worker> Workers { get; set; } = new HashSet<Worker>();
        public virtual ICollection<WorkerEducation> WorkerEducations { get; set; } = new HashSet<WorkerEducation>();
    }
}






