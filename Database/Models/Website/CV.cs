using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("CVs")]
    public class CV
    {
        [Key]
        public int CVId { get; set; }

        public int WorkerId { get; set; }

        [Required]
        [StringLength(200)]
        public string CVName { get; set; }

        [StringLength(50)]
        public string Template { get; set; } = "default";

        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = false;
        public int Views { get; set; } = 0;
        public int Downloads { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }

        public virtual ICollection<CVSection> Sections { get; set; } = new HashSet<CVSection>();
    }
}









