using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("DashboardStats")]
    public class DashboardStats
    {
        [Key]
        public int StatId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string StatType { get; set; } // JobViews, Applications, SavedJobs, etc.

        [Required]
        [StringLength(100)]
        public string StatValue { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StatDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
    }
}
