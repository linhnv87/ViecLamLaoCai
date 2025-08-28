using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("CVSections")]
    public class CVSection
    {
        [Key]
        public int SectionId { get; set; }

        public int CVId { get; set; }

        [Required]
        [StringLength(50)]
        public string SectionType { get; set; } // PersonalInfo, Education, Experience, Skills, etc.

        [Required]
        public string SectionData { get; set; } // JSON data for the section

        public int OrderNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CVId")]
        public virtual CV CV { get; set; }
    }
}









