using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Communes")]
    public class Commune
    {
        [Key]
        public int CommuneId { get; set; }

        [Required]
        [StringLength(100)]
        public string CommuneName { get; set; }

        public int DistrictId { get; set; }

        [StringLength(20)]
        public string? CommuneCode { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }

        public virtual ICollection<Worker> Workers { get; set; } = new HashSet<Worker>();
        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();
    }
}









