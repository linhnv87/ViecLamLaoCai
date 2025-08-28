using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // JobApplication, Interview, System, etc.

        public bool IsRead { get; set; } = false;

        [StringLength(50)]
        public string? RelatedEntityType { get; set; } // JobPosting, JobApplication, Interview, etc.

        public int? RelatedEntityId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReadDate { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
    }
}
