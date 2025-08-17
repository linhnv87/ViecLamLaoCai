using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicApi.Models.AppRole;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/roles")]
[ApiController]
public class AppRoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AppRoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAllRole()
    {
        var result = await _roleManager.Roles.ToListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] AppRoleCreateVModelRequest model)
    {
        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
        if (roleExists)
        {
            return BadRequest(new BaseResponseModel("Role already exists"));
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
        if (result.Succeeded)
        {
            return Ok(new BaseResponseModel("Role created successfully"));
        }

        return BadRequest(new BaseResponseModel("Error creating role"));
    }
}
