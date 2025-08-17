using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryStatusesController : ControllerBase
    {
        private readonly ICategoryStatusesService _categoryStatusesService;

        public CategoryStatusesController(ICategoryStatusesService categoryStatusesService)
        {
            _categoryStatusesService = categoryStatusesService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var result = _categoryStatusesService.GetAllStatuses();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
