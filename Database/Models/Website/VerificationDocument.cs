using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("VerificationDocuments")]
    public class VerificationDocument
    {
        [Key]
        public int Id { get; set; }
        
        public int ApprovalId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }
        
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }
        
        public long FileSize { get; set; }
        
        [Required]
        [StringLength(10)]
        public string FileType { get; set; }
        
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("ApprovalId")]
        public virtual BusinessApproval BusinessApproval { get; set; }
    }
}
