using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToTrinhController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IToTrinhService _toTrinhService;

        public ToTrinhController(IDocumentService documentService, IToTrinhService toTrinhService)
        {
            _documentService = documentService;
            _toTrinhService = toTrinhService;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ToTrinhUpdateRequestDTO payload)
        {
            var result = await _toTrinhService.UpdateAsync(payload);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> ReUpdate([FromForm] ToTrinhUpdateRequestDTO payload)
        {
            var result = await _toTrinhService.ReUpdateAsync(payload);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetTotalWorkFlowByUser([FromForm] Guid userId)
        {
            var result = await _toTrinhService.GetTotalWorkFlowByUser(userId);
            return Ok(result);
        }
    }
}
