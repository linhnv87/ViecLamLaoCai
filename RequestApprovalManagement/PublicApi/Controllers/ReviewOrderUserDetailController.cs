using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.ReviewOrderUserDetail;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/review-order/users")]
[ApiController]
public class ReviewOrderUserDetailController : ControllerBase
{
    private readonly IRepository<ReviewOrderUserDetailEntity> _repository;

    public ReviewOrderUserDetailController(
        IRepository<ReviewOrderUserDetailEntity> repository)
    {
        _repository = repository;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAllRole()
    {
        var result = await _repository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReviewOrderUserDetailCreateVModelRequest model)
    {
        var result = await _repository.AddAsync(new ReviewOrderUserDetailEntity
        {
            ReviewOrderId = model.ReviewOrderId,
            UserId = model.UserId,
            IsDefault = model.IsDefault
        });
        return Ok(new BaseResponseModel(result));
    }
}
