using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class SignUpDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public string PasswordQuestion { get; set; }
        //public string PasswordAnswer { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
        public List<int> FieldIds { get; set; } = new List<int>();
        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }
    }

    public class LogInDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDTO
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResponseTokenDTO
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public string AccessToken { get; set; }        
        public string DisplayName { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string SelectedRole { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<int> FieldIds { get;set; } = new List<int>();
    }
    public class UserInfoDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }        
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime LastloginDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = true;
        public bool IsLockedout { get; set; } = false;
        public List<RoleDTO>? Roles { get; set; }
        public List<int>? FieldIds { get; set; }
        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }
    }
    public class RoleDTO
    {
        public Guid RoleId { get; set;}
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool? Deleted { get; set; }
    }
}
