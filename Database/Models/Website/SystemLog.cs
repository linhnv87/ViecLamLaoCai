using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("SystemLogs")]
    public class SystemLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        [StringLength(20)]
        public string LogLevel { get; set; } // Info, Warning, Error, Fatal

        [Required]
        [StringLength(1000)]
        public string LogMessage { get; set; }

        public string? Exception { get; set; }

        [StringLength(100)]
        public string? Source { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}









