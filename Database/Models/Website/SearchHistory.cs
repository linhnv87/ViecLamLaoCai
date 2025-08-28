using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("SearchHistory")]
    public class SearchHistory
    {
        [Key]
        public int HistoryId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string SearchKeyword { get; set; }

        [StringLength(1000)]
        public string? SearchFilters { get; set; }

        public DateTime SearchDate { get; set; } = DateTime.Now;
        public int? ResultCount { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }
    }
}
