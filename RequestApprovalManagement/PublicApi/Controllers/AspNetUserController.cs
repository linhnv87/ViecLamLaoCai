using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.AspNetUser;

namespace PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AspNetUserController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenClaimsService _tokenClaimsService;

    public AspNetUserController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ITokenClaimsService tokenClaimsService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenClaimsService = tokenClaimsService;
    }

    [AllowAnonymous]
    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp(SignUpRequest payload)
    {
        var user = new ApplicationUser
        { 
            UserName = payload.Username,
            Email = payload.Email,
            NormalizedUserName = payload.FullName,
            PhoneNumber = payload.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, payload.Password);

        return Ok(new BaseResponseModel(result));
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);

        var response = new AuthenticateResponse()
        {
            UserName = request.Username
        };

        if (result.Succeeded)
        {
            response.AccessToken = await _tokenClaimsService.GetTokenAsync(request.Username);
        }

        return Ok(new BaseResponseModel(response));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetUsersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new List<AspNetUserWithRolesResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new AspNetUserWithRolesResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = (List<string>)roles
            });
        }

        return Ok(new BaseResponseModel(userRoles));
    }
}
