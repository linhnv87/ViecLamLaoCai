using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost("CreateComment")]
        public async Task<IActionResult> CreateComment(CommentDTO payload)
        {
            try
            {
                var result = await commentService.CreateComment(payload);
                return Ok(new BaseResponseModel(result));

            }catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetAllComment")]
        public async Task<IActionResult> GetAllComments()
        {
            try
            {
                var result = await commentService.GetAllComments();
                return Ok(new BaseResponseModel(result));

            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetDocumentComments/{docId}")]
        public async Task<IActionResult> GetDocumentComments(int docId)
        {
            try
            {
                var result = await commentService.GetDocumentComments(docId);
                return Ok(new BaseResponseModel(result));

            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
