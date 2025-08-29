using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using Services.DTO;
using Core.Domains;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QuanLyToTrinh.Controllers.Website
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessApprovalController : ControllerBase
    {
        private readonly IBusinessApprovalService _businessApprovalService;

        public BusinessApprovalController(IBusinessApprovalService businessApprovalService)
        {
            _businessApprovalService = businessApprovalService;
        }

        /// <summary>
        /// Get paginated list of business approvals with filtering
        /// </summary>
        [HttpPost("GetBusinessApprovals")]
        public async Task<IActionResult> GetBusinessApprovals([FromBody] BusinessApprovalFilterDTO filter)
        {
            try
            {
                var result = await _businessApprovalService.GetBusinessApprovalsAsync(filter);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get business approval statistics
        /// </summary>
        [HttpGet("GetStatistics")]
        public async Task<IActionResult> GetApprovalStatistics()
        {
            try
            {
                var result = await _businessApprovalService.GetApprovalStatisticsAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get business approval detail by ID
        /// </summary>
        [HttpGet("GetById/{businessId}")]
        public async Task<IActionResult> GetBusinessApprovalById(int businessId)
        {
            try
            {
                var result = await _businessApprovalService.GetBusinessApprovalByIdAsync(businessId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Approve a business registration
        /// </summary>
        [HttpPut("Approve")]
        public async Task<IActionResult> ApproveBusiness([FromBody] ApproveBusinessRequestDTO request)
        {
            try
            {
                var result = await _businessApprovalService.ApproveBusinessAsync(request);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Reject a business registration
        /// </summary>
        [HttpPut("Reject")]
        public async Task<IActionResult> RejectBusiness([FromBody] RejectBusinessRequestDTO request)
        {
            try
            {
                var result = await _businessApprovalService.RejectBusinessAsync(request);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Set business approval status to reviewing
        /// </summary>
        [HttpPut("SetReviewing")]
        public async Task<IActionResult> SetReviewingBusiness([FromBody] SetReviewingRequestDTO request)
        {
            try
            {
                var result = await _businessApprovalService.SetReviewingBusinessAsync(request);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get business documents by business ID
        /// </summary>
        [HttpGet("GetDocuments/{businessId}")]
        public async Task<IActionResult> GetBusinessDocuments(int businessId)
        {
            try
            {
                var result = await _businessApprovalService.GetBusinessDocumentsAsync(businessId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Download business document by document ID
        /// </summary>
        [HttpGet("DownloadDocument/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                var (fileContent, fileName, contentType) = await _businessApprovalService.DownloadDocumentAsync(documentId);
                return File(fileContent, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Verify a business document
        /// </summary>
        [HttpPut("VerifyDocument/{documentId}")]
        public async Task<IActionResult> VerifyDocument(int documentId, [FromBody] string verifiedBy)
        {
            try
            {
                var result = await _businessApprovalService.VerifyDocumentAsync(documentId, verifiedBy);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get approval history for a business
        /// </summary>
        [HttpGet("GetHistory/{businessId}")]
        public async Task<IActionResult> GetApprovalHistory(int businessId)
        {
            try
            {
                var result = await _businessApprovalService.GetApprovalHistoryAsync(businessId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get businesses by status
        /// </summary>
        [HttpGet("GetByStatus/{status}")]
        public async Task<IActionResult> GetBusinessesByStatus(string status)
        {
            try
            {
                var result = await _businessApprovalService.GetBusinessesByStatusAsync(status);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Search businesses by keyword
        /// </summary>
        [HttpGet("Search")]
        public async Task<IActionResult> SearchBusinesses([FromQuery] string keyword)
        {
            try
            {
                var result = await _businessApprovalService.SearchBusinessesAsync(keyword);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get today's submissions count
        /// </summary>
        [HttpGet("GetTodaySubmissions")]
        public async Task<IActionResult> GetTodaySubmissionsCount()
        {
            try
            {
                var result = await _businessApprovalService.GetTodaySubmissionsCountAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Bulk approve multiple businesses
        /// </summary>
        [HttpPut("BulkApprove")]
        public async Task<IActionResult> BulkApproveBusinesses([FromBody] BulkApproveRequestDTO request)
        {
            try
            {
                var result = await _businessApprovalService.BulkApproveBusinessesAsync(request.BusinessIds, request.ApprovedBy);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Bulk reject multiple businesses
        /// </summary>
        [HttpPut("BulkReject")]
        public async Task<IActionResult> BulkRejectBusinesses([FromBody] BulkRejectRequestDTO request)
        {
            try
            {
                var result = await _businessApprovalService.BulkRejectBusinessesAsync(request.BusinessIds, request.RejectedBy, request.Reason);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Export business approvals to Excel/PDF
        /// </summary>
        [HttpPost("Export")]
        public async Task<IActionResult> ExportBusinessApprovals([FromBody] ExportBusinessApprovalsRequestDTO request)
        {
            try
            {
                var (fileContent, fileName, contentType) = await _businessApprovalService.ExportBusinessApprovalsAsync(request.Filter, request.Format);
                return File(fileContent, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
