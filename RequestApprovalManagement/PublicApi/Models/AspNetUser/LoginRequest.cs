using System.ComponentModel.DataAnnotations;

namespace PublicApi.Models.AspNetUser;

public class LoginRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
