using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/GetReviewerWorkflow")]
    [ApiController]
    public class WorkFlowController : ControllerBase
    {
        private readonly ICfgWorkFlowService _service;
        public WorkFlowController(ICfgWorkFlowService reviewOrderService)
        {
            _service = reviewOrderService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetByUserAsync([FromBody] WorkFlowRequestDTO request)
        {
            var result = await _service.GetWorkFlowByUser(request);

            if (result == null)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status404NotFound, "Not found"));
            }

            return Ok(new BaseResponseModel(result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkFlowAsync([FromBody] ReviewOrderByIdDTO request)
        {
            var result = await _service.CreateWorkflowAsync(request);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new BaseResponseModel(null, false, StatusCodes.Status409Conflict, "Đã có cấu hình luồng này rồi"));
            }

            return Ok(new BaseResponseModel(result));
        }
        [HttpGet("GetGetUsersByWorkflowId/{workflowId}")]
        public async Task<IActionResult> GetGetUsersByWorkflowId(int workflowId)
        {
            try
            {
                var result = await _service.GetUsersByWorkflowId(workflowId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpPut("{workflowId:int}")]
        public async Task<IActionResult> Update([FromRoute] int workflowId, [FromBody] UpdateWorkflowRequestDTO payload)
        {
            try
            {
                var result = await _service.UpdateWorkflowAndUserAsync(workflowId, payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new BaseResponseModel(null, false, StatusCodes.Status409Conflict, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new BaseResponseModel(null, false, StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteWorkflowAndUsersAsync(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

    }
}
