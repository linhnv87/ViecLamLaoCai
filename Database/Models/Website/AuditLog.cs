using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } // Create, Update, Delete, View

        [Required]
        [StringLength(100)]
        public string TableName { get; set; }

        public int? RecordId { get; set; }

        public string? OldValues { get; set; } // JSON string of old values
        public string? NewValues { get; set; } // JSON string of new values

        [StringLength(45)]
        public string? IPAddress { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }
    }
}
