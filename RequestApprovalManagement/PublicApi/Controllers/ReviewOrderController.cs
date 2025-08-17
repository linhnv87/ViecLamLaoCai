using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.ReviewOrder;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/review-order")]
[ApiController]
public class ReviewOrderController : ControllerBase
{
    private readonly IRepository<ReviewOrderEntity> _reviewOrderRepository;

    public ReviewOrderController(
        IRepository<ReviewOrderEntity> reviewOrderRepository)
    {
        _reviewOrderRepository = reviewOrderRepository;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAllRole()
    {
        var result = await _reviewOrderRepository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReviewOrderCreateVModelRequest model)
    {
        var result = await _reviewOrderRepository.AddAsync(new ReviewOrderEntity
        {
            Name = model.Name,
            PrevId = model.PrevId,
            NextId = model.NextId,
            DefaultUserId = model.DefaultUserId,
            IsSign = model.IsSign,
            Description = model.Description
        });
        return Ok(new BaseResponseModel(result));
    }
}
