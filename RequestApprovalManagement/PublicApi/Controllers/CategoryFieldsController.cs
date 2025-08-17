using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Models.CreateVModel;




namespace PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryFieldsController : ControllerBase
{
    private readonly IRepository<CategoryFieldsEntity> _fielsRepository;
    public CategoryFieldsController(
        IRepository<CategoryFieldsEntity> fielsRepository)
    {
        _fielsRepository = fielsRepository;
    }
    [Route("search")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _fielsRepository.ListAsync();
        return Ok(new BaseResponseModel(result));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateVModelRequest request)
    {
        var result = await _fielsRepository.AddAsync(new CategoryFieldsEntity
        {
            Name = request.Name,
            Description = request.Description
        });
        return Ok(new BaseResponseModel(result));
    }
}
