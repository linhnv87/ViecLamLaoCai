using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.ReviewProcess;

namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/review-process")]
[ApiController]
public class ReviewProcessController : ControllerBase
{
    private readonly IRepository<ReviewProcessEntity> _reviewProcessRepository;

    public ReviewProcessController(
        IRepository<ReviewProcessEntity> reviewProcessRepository)
    {
        _reviewProcessRepository = reviewProcessRepository;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reviewProcessRepository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReviewProcessCreateVModelRequest model)
    {
        var result = await _reviewProcessRepository.AddAsync(new ReviewProcessEntity
        {
            CreatedBy = model.CreatedBy,
            DocumentId = model.DocumentId,
            ReviewDate = model.ReviewDate,
            DocumentStatus = model.DocumentStatus,
            Comments = model.Comments,
            Deadline = model.Deadline
        });
        return Ok(new BaseResponseModel(result));
    }
}
