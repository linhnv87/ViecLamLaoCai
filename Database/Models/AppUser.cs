using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models.Website;

namespace Database.Models
{
    [Table("AppUsers")]
    public class AppUser
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(200)]
        public string UserFullName { get; set; }

        [Required]
        [StringLength(500)]
        public string HashedPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = true;
        public bool IsLockedout { get; set; } = false;

        [StringLength(100)]
        public string? FieldOfficer { get; set; }

        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }

        // ===== NAVIGATION PROPERTIES CHO WEBSITE VIỆC LÀM =====

        // Worker (nếu user này là người lao động)
        public virtual Worker? Worker { get; set; }

        // Company (nếu user này là chủ doanh nghiệp)
        public virtual Company? Company { get; set; }

        // Notifications
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

        // Search History
        public virtual ICollection<SearchHistory> SearchHistory { get; set; } = new HashSet<SearchHistory>();

        // Dashboard Stats
        public virtual ICollection<DashboardStats> DashboardStats { get; set; } = new HashSet<DashboardStats>();

        // User Activity
        public virtual ICollection<UserActivity> UserActivity { get; set; } = new HashSet<UserActivity>();

        // Audit Logs
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new HashSet<AuditLog>();

        // Saved Jobs (nếu là worker)
        public virtual ICollection<SavedJob> SavedJobs { get; set; } = new HashSet<SavedJob>();

        // Job Applications (nếu là worker)
        public virtual ICollection<JobApplication> JobApplications { get; set; } = new HashSet<JobApplication>();

        // AppUserInRole relationships
        public virtual ICollection<AppUserInRole> AppUsersInRoles { get; set; } = new HashSet<AppUserInRole>();
    }

    [Table("AppRoles")]
    public class AppRole
    {
        [Key]
        public Guid RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool Active { get; set; } = true;
        public bool? Deleted { get; set; }

        // Navigation Properties
        public virtual ICollection<AppUserInRole> AppUsersInRoles { get; set; } = new HashSet<AppUserInRole>();
    }

    [Table("AppUsersInRoles")]
    public class AppUserInRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("RoleId")]
        public virtual AppRole AppRole { get; set; }
    }

    public class AppUserRoles
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;
    }
}
