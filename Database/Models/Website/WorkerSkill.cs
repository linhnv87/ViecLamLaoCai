using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("WorkerSkills")]
    public class WorkerSkill
    {
        [Key]
        public int SkillId { get; set; }

        public int WorkerId { get; set; }

        [Required]
        [StringLength(100)]
        public string SkillName { get; set; }

        [StringLength(20)]
        public string Level { get; set; } // Beginner, Intermediate, Advanced, Expert

        public int YearsOfExperience { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }
    }
}









