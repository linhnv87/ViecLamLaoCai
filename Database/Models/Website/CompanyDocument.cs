using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("CompanyDocuments")]
    public class CompanyDocument
    {
        [Key]
        public int DocumentId { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } // Business License, Tax Code, Certificate, Other

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; }

        [Required]
        [StringLength(500)]
        public string DocumentUrl { get; set; }

        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }
}









