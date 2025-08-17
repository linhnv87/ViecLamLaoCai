using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _service;

        public GroupController(IGroupService service)
        {
            _service = service;
        }

        [HttpGet("GetAllGroups")]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var result = await _service.GetAllGroupsAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetAllUsersByGroupId(int groupId)
        {
            try
            {
                var result = await _service.GetAllInfoUserByGroupId(groupId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] GroupDTO payload)
        {
            try
            {
                var result = await _service.CreateGroupAsync(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("{groupId:int}")]
        public async Task<IActionResult> Update([FromRoute] int groupId, [FromBody] GroupDTO payload)
        {
            try
            {
                var result = await _service.UpdateGroupAsync(groupId, payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPatch("{groupId:int}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int groupId)
        {
            try
            {
                var result = await _service.UpdateStatusGroupAsync(groupId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpDelete("{groupId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int groupId)
        {
            try
            {
                var result = await _service.DeleteGroupAsync(groupId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}