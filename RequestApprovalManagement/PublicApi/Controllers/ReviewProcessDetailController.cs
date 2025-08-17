using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.ReviewProcessDetail;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/review-process-detail")]
[ApiController]
public class ReviewProcessDetailController : ControllerBase
{
    private readonly IRepository<ReviewProcessDetailEntity> _reviewProcessDetailRepository;

    public ReviewProcessDetailController(
        IRepository<ReviewProcessDetailEntity> reviewProcessDetailRepository)
    {
        _reviewProcessDetailRepository = reviewProcessDetailRepository;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reviewProcessDetailRepository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReviewProcessDetailCreateVModelRequest model)
    {
        var result = await _reviewProcessDetailRepository.AddAsync(new ReviewProcessDetailEntity
        {
            CurrentProcessId = model.CurrentProcessId,
            ReviewProcessId = model.ReviewProcessId,
            ProcessStatus = model.ProcessStatus,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            ResultLinkDocumentId = model.ResultLinkDocumentId,
            Deadline = model.Deadline
        });
        return Ok(new BaseResponseModel(result));
    }
}
