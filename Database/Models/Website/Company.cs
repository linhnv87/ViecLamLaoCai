using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Website
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Industry { get; set; }

        [StringLength(50)]
        public string CompanySize { get; set; }

        [StringLength(200)]
        public string Website { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(20)]
        public string TaxNumber { get; set; }

        [StringLength(100)]
        public string Position { get; set; }

        public bool IsVerified { get; set; } = false;

        [StringLength(20)]
        public string ApprovalStatus { get; set; } = "Pending";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        // Location
        public int? DistrictId { get; set; }
        public int? CommuneId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }

        [ForeignKey("CommuneId")]
        public virtual Commune Commune { get; set; }

        // Collections
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new HashSet<JobPosting>();
        public virtual ICollection<CompanyDocument> Documents { get; set; } = new HashSet<CompanyDocument>();
        public virtual ICollection<BusinessApproval> BusinessApprovals { get; set; } = new HashSet<BusinessApproval>();
    }
}