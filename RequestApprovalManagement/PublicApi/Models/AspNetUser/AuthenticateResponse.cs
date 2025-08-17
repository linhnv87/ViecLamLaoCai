namespace PublicApi.Models.AspNetUser;

public class AuthenticateResponse
{
    public AuthenticateResponse()
    {
        
    }

    public string UserName { get; set; }
    public string AccessToken { get; set; }
    public string DisplayName { get; set; }
    public DateTime TokenExpiration { get; set; }
    public string SelectedRole { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public List<int> FieldIds { get; set; } = new List<int>();
}
