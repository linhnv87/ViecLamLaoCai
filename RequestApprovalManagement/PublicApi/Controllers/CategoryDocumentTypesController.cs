using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.CreateVModel;

namespace PublicApi.Controllers;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

[Route("api/documenttype")]
[ApiController]
public class CategoryDocumentTypesController : ControllerBase
{
    private readonly IRepository<CategoryDocumentTypesEntity> _documetTypeEntity;
    public CategoryDocumentTypesController(
      IRepository<CategoryDocumentTypesEntity> documetTypeEntity)
    {
        _documetTypeEntity = documetTypeEntity;
    }
    [Route("search")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _documetTypeEntity.ListAsync();
        return Ok(new BaseResponseModel(result));
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateVModelRequest request)
    {
        var result = await _documetTypeEntity.AddAsync(new CategoryDocumentTypesEntity
        {
            Name = request.Name,
            Description = request.Description
        });
        return Ok(new BaseResponseModel(result));
    }
}
