﻿namespace PublicApi.Models.AspNetUser;

public class AspNetUserWithRolesResponse
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string>? Roles { get; set; }
}
