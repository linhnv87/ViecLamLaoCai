using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("UserActivity")]
    public class UserActivity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; } // Login, JobView, Application, etc.

        [Required]
        [StringLength(500)]
        public string ActivityDescription { get; set; }

        [StringLength(50)]
        public string? RelatedEntityType { get; set; } // JobPosting, Company, etc.

        public int? RelatedEntityId { get; set; }

        [StringLength(45)]
        public string? IPAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
    }
}
