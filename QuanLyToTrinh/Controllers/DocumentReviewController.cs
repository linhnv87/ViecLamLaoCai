using Aspose.Pdf.Devices;
using Core.Domains;
using Core.QueryModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyToTrinh.SMSService;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentReviewController : ControllerBase
    {
        private readonly IDocumentReviewService documentReviewService;
        private readonly IDocumentService documentService;
        private readonly ISMSService smsService;

        public DocumentReviewController(IDocumentReviewService documentReviewService, IDocumentService documentService, ISMSService smsService)
        {
            this.documentReviewService = documentReviewService;
            this.documentService = documentService;
            this.smsService = smsService;
        }

        [HttpPut("UpdateDocumentReview")]
        public async Task<IActionResult> UpdateDocumentReview([FromForm] DocumentReviewDTO payload)
        {

            try
            {
                //var mesRes = await documentService.UpdateApprovalStatus_V2();
              
                var result = await documentReviewService.UpdateDocumentReview(payload);

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpPut("UpdateDocumentApproval")]
        public async Task<IActionResult> UpdateDocumentApproval(DocumentReviewDTO payload)
        {

            try
            {
                var result = await documentReviewService.UpdateDocumentApprovalAsync(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
        [HttpGet("GetItemByUserId/{userId}")]
        public async Task<IActionResult> GetItemByUserId(Guid userId)
        {
            try
            {
                var result = await documentReviewService.GetItemByUserIdAsync(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
        [HttpGet("GetFinalPdf/{docid}")]
        public async Task<IActionResult> GetFinalPdf(int docid)
        {
            try
            {
                var finalPdf = await documentService.GetFinalPdf(docid);
                return Ok(new BaseResponseModel(finalPdf));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDocumentReview(DocumentReadFileDTO payload)
        {
            try
            {
                var result = await documentReviewService.DocumentReadFileAsync(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetSingleByUserIdAndDocId/{userId}/{docId}")]
        public async Task<IActionResult> GetSingleByUserIdAndDocId(Guid userId, int docId)
        {
            try
            {
                var result = await documentReviewService.GetSingleByUserIdAndDocId(userId, docId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetApprovalSummary/{id}")]
        public async Task<IActionResult> GetApprovalSummary(int id)
        {
            try
            {
                var result = await documentReviewService.GetApprovalSummary(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpPut("GetApprovalSummary_V2")]
        public async Task<IActionResult> GetApprovalSummary_V2(DocumentSummaryQueryModel queries)
        {
            try
            {
                var result = await documentReviewService.GetApprovalSummary_V2(queries);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetAllDocumentApproval")]
        public async Task<IActionResult> GetAllDocumentApproval()
        {
            try
            {
                var result = await documentReviewService.GetAllDocumentsApprovalAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetIndividualDocumentApprovals/{userId}")]
        public async Task<IActionResult> GetIndividualDocumentApprovals(Guid userId)
        {
            try
            {
                var result = await documentReviewService.GetIndividualApprovalList(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
    }
}
