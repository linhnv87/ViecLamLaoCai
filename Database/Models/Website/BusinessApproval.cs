using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("BusinessApprovals")]
    public class BusinessApproval
    {
        [Key]
        public int ApprovalId { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [StringLength(20)]
        public string ApprovalStatus { get; set; } = "pending"; // pending, approved, rejected, cancelled

        [StringLength(100)]
        public string ApprovedBy { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovalDate { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string VerificationCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        
        public virtual ICollection<VerificationDocument> VerificationDocuments { get; set; }
    }
}
