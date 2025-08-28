using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("EmailTemplates")]
    public class EmailTemplate
    {
        [Key]
        public int TemplateId { get; set; }

        [Required]
        [StringLength(200)]
        public string TemplateName { get; set; }

        [Required]
        [StringLength(50)]
        public string TemplateCode { get; set; }

        [Required]
        [StringLength(500)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [StringLength(1000)]
        public string? Variables { get; set; } // JSON string of available variables

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<EmailLog> EmailLogs { get; set; } = new HashSet<EmailLog>();
    }
}









