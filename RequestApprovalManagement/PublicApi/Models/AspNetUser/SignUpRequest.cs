using System.ComponentModel.DataAnnotations;

namespace PublicApi.Models.AspNetUser;

public class SignUpRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PhoneNumber { get; set; }
    //public string PasswordQuestion { get; set; }
    //public string PasswordAnswer { get; set; }
    public List<string> RoleIds { get; set; } = new List<string>();
    public List<int> FieldIds { get; set; } = new List<int>();
}
