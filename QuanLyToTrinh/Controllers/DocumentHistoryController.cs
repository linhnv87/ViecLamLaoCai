using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;

using Services;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentHistoryController : ControllerBase
    {
        private readonly IDocumentHistoryService _service;
        public DocumentHistoryController(IDocumentHistoryService fileService)
        {
            _service = fileService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAll();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetDocumentHistory/{docId}")]
        public async Task<IActionResult> GetDocumentHistory(int docId)
        {
            try
            {
                var result = await _service.GetDocumentHistory(docId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetDocumentHistoryByUser/{userId}")]
        public async Task<IActionResult> GetDocumentHistoryByUser(string userId)
        {
            try
            {
                var result = await _service.GetDocumentHistoryByUser(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetById(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create( DocumentHistoryDTO payload)
        {
            try
            {
                var result = await _service.Create(payload);

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }


        //[HttpPut("Update")]
        //public async Task<IActionResult> Update(DocumentHistoryDTO payload)
        //{
        //    try
        //    {
        //        var result = await _service.Update(payload);
        //        return Ok(new BaseResponseModel(result));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        //    }
        //}

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.Delete(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }

}
