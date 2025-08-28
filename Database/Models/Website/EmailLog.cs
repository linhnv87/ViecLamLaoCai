using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("EmailLogs")]
    public class EmailLog
    {
        [Key]
        public int LogId { get; set; }

        public int TemplateId { get; set; }

        [Required]
        [StringLength(200)]
        [EmailAddress]
        public string RecipientEmail { get; set; }

        [Required]
        [StringLength(500)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Sent"; // Sent, Failed, Pending

        public DateTime SentDate { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        // Navigation Properties
        [ForeignKey("TemplateId")]
        public virtual EmailTemplate EmailTemplate { get; set; }
    }
}









