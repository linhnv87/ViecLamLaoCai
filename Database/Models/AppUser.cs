using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public class AppUser
    {
        [Key]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string HashedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = true;
        public bool IsLockedout { get; set; } = false;
        public string? FieldOfficer { get; set; }
        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }
    }

    public class AppRole
    {
        [Key]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;
        public bool? Deleted { get; set; }
    }

    public class AppUserInRole
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }        
        public Guid RoleId { get; set; }
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
