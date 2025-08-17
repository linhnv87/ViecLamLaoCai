using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities;

public class AppUser : IAggregateRoot
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
}

public class AppRole : IAggregateRoot
{
    [Key]
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; } = true;
}

public class AppUserInRole : IAggregateRoot
{
    [Key]
    public int Id { get; set; }
    public Guid UserId { get; set; }        
    public Guid RoleId { get; set; }
}
