using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.ReviewUserDetail;


namespace PublicApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/review-user-detail")]
[ApiController]
public class ReviewUserDetailController : ControllerBase
{
    private readonly IRepository<ReviewUserDetailEntity> _reviewUserDetailRepository;

    public ReviewUserDetailController(
        IRepository<ReviewUserDetailEntity> reviewUserDetailRepository)
    {
        _reviewUserDetailRepository = reviewUserDetailRepository;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reviewUserDetailRepository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReviewUserDetailCreateVModelRequest model)
    {
        var result = await _reviewUserDetailRepository.AddAsync(new ReviewUserDetailEntity
        {
            ReviewProcessDetailId = model.ReviewProcessDetailId,
            UserId = model.UserId,
            ProcessStatus = model.ProcessStatus,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            SignAt = model.SignAt,
            SignedLinkDocument = model.SignedLinkDocument,
            ResultLinkDocumentId = model.ResultLinkDocumentId,
            Comments = model.Comments
        });
        return Ok(new BaseResponseModel(result));
    }
}
