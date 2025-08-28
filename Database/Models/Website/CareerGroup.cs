using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("CareerGroups")]
    public class CareerGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        [StringLength(20)]
        public string? GroupCode { get; set; }

        public int? FieldId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }

        public virtual ICollection<Specialization> Specializations { get; set; } = new HashSet<Specialization>();
    }
}









