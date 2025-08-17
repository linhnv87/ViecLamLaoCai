using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.CreateVModel;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/statuses")]
[ApiController]
public class CategoryStatusesController : ControllerBase
{
    private readonly IRepository<CategoryStatusesEntity> _statusRepository;

    public CategoryStatusesController(
        IRepository<CategoryStatusesEntity> statusRepository)
    {
        _statusRepository = statusRepository;
    }

    [Route("search")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _statusRepository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateVModelRequest request)
    {
        var result = await _statusRepository.AddAsync(new CategoryStatusesEntity
        {
            Name = request.Name,
            Description = request.Description
        });
        return Ok(new BaseResponseModel(result));
    }
}
