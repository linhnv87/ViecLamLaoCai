using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.ReviewOrderGroupDetail;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/review-order/group")]
[ApiController]
public class ReviewOrderGroupDetailController : ControllerBase
{
    private readonly IRepository<ReviewOrderGroupDetailEntity> _repository;

    public ReviewOrderGroupDetailController(
        IRepository<ReviewOrderGroupDetailEntity> repository)
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
    public async Task<IActionResult> Post([FromBody] ReviewOrderGroupDetailCreateVModelRequest model)
    {
        var result = await _repository.AddAsync(new ReviewOrderGroupDetailEntity
        {
            ReviewOrderId = model.ReviewOrderId,
            RoleId = model.RoleId,
            DefaultUserId = model.DefaultUserId,
            IsDefault = model.IsDefault
        });
        return Ok(new BaseResponseModel(result));
    }
}
