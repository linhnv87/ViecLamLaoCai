using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using Services.DTO;
using Core.Domains;
using System;
using System.Threading.Tasks;

namespace QuanLyToTrinh.Controllers.Website
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessVerificationController : ControllerBase
    {
        private readonly IBusinessVerificationService _businessVerificationService;

        public BusinessVerificationController(IBusinessVerificationService businessVerificationService)
        {
            _businessVerificationService = businessVerificationService;
        }

        /// <summary>
        /// Submit business verification with documents
        /// </summary>
        [HttpPost("SubmitVerification")]
        public async Task<IActionResult> SubmitVerification([FromForm] BusinessVerificationRequestDTO request)
        {
            try
            {
                var result = await _businessVerificationService.SubmitVerificationAsync(request);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get verification status for a company
        /// </summary>
        [HttpGet("GetStatus/{companyId}")]
        public async Task<IActionResult> GetVerificationStatus(int companyId)
        {
            try
            {
                var result = await _businessVerificationService.GetVerificationStatusAsync(companyId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get verification status by user ID
        /// </summary>
        [HttpGet("GetStatusByUserId/{userId}")]
        public async Task<IActionResult> GetVerificationStatusByUserId(Guid userId)
        {
            try
            {
                var result = await _businessVerificationService.GetVerificationStatusByUserIdAsync(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get verification history for a company
        /// </summary>
        [HttpGet("GetHistory/{companyId}")]
        public async Task<IActionResult> GetVerificationHistory(int companyId)
        {
            try
            {
                var result = await _businessVerificationService.GetVerificationHistoryAsync(companyId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Check if company is verified by company ID
        /// </summary>
        [HttpGet("IsVerified/{companyId}")]
        public async Task<IActionResult> IsCompanyVerified(int companyId)
        {
            try
            {
                var result = await _businessVerificationService.IsCompanyVerifiedAsync(companyId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Check if company is verified by user ID
        /// </summary>
        [HttpGet("IsVerifiedByUserId/{userId}")]
        public async Task<IActionResult> IsVerifiedByUserId(Guid userId)
        {
            try
            {
                var result = await _businessVerificationService.IsCompanyVerifiedByUserIdAsync(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Resend verification email
        /// </summary>
        [HttpPost("ResendEmail/{verificationId}")]
        public async Task<IActionResult> ResendVerificationEmail(int verificationId)
        {
            try
            {
                var result = await _businessVerificationService.ResendVerificationEmailAsync(verificationId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Cancel pending verification
        /// </summary>
        [HttpDelete("CancelVerification/{verificationId}")]
        public async Task<IActionResult> CancelVerification(int verificationId)
        {
            try
            {
                var result = await _businessVerificationService.CancelVerificationAsync(verificationId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Get company information by user ID
        /// </summary>
        [HttpGet("GetCompanyInfo/{userId}")]
        public async Task<IActionResult> GetCompanyInfo(Guid userId)
        {
            try
            {
                var result = await _businessVerificationService.GetCompanyInfoByUserIdAsync(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
