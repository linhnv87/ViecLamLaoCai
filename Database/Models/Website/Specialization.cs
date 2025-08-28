using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Specializations")]
    public class Specialization
    {
        [Key]
        public int SpecializationId { get; set; }

        [Required]
        [StringLength(100)]
        public string SpecializationName { get; set; }

        [StringLength(20)]
        public string? SpecializationCode { get; set; }

        public int? GroupId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("GroupId")]
        public virtual CareerGroup? CareerGroup { get; set; }

        public virtual ICollection<Career> Careers { get; set; } = new HashSet<Career>();
    }
}









