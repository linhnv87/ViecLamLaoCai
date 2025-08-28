using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace QuanLyToTrinh.Controllers.Website
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("GetBusinessDashboard/{userId}")]
        public async Task<IActionResult> GetBusinessDashboard(string userId)
        {
            try
            {
                var result = await _dashboardService.GetBusinessDashboardByUserIdAsync(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetCandidateDashboard/{candidateId}")]
        public async Task<IActionResult> GetCandidateDashboard(string candidateId)
        {
            try
            {
                var result = await _dashboardService.GetCandidateDashboardAsync(candidateId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetAdminDashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var result = await _dashboardService.GetAdminDashboardAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetRecentActivities/{userId}")]
        public async Task<IActionResult> GetRecentActivities(string userId)
        {
            try
            {
                var result = await _dashboardService.GetRecentActivitiesAsync(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // ===== DETAILED BUSINESS DASHBOARD APIs =====

        [HttpGet("GetRecentJobs/{businessId}")]
        public async Task<IActionResult> GetRecentJobs(int businessId, [FromQuery] string companyName)
        {
            try
            {
                var result = await _dashboardService.GetRecentJobsAsync(businessId, companyName);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetRecentCandidates/{businessId}")]
        public async Task<IActionResult> GetRecentCandidates(int businessId)
        {
            try
            {
                var result = await _dashboardService.GetRecentCandidatesAsync(businessId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // ===== DETAILED CANDIDATE DASHBOARD APIs =====

        [HttpGet("GetSavedJobs/{candidateId}")]
        public async Task<IActionResult> GetSavedJobs(string candidateId)
        {
            try
            {
                var result = await _dashboardService.GetSavedJobsAsync(candidateId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetRecentApplications/{candidateId}")]
        public async Task<IActionResult> GetRecentApplications(string candidateId)
        {
            try
            {
                var result = await _dashboardService.GetRecentApplicationsAsync(candidateId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetAppliedJobs/{candidateId}")]
        public async Task<IActionResult> GetAppliedJobs(string candidateId)
        {
            try
            {
                var result = await _dashboardService.GetAppliedJobsAsync(candidateId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // ===== ADMIN DASHBOARD DETAIL APIs =====

        [HttpGet("GetTotalBusinesses")]
        public async Task<IActionResult> GetTotalBusinesses()
        {
            try
            {
                var result = await _dashboardService.GetTotalBusinessesAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetPendingApprovals")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            try
            {
                var result = await _dashboardService.GetPendingApprovalsAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetApprovedBusinesses")]
        public async Task<IActionResult> GetApprovedBusinesses()
        {
            try
            {
                var result = await _dashboardService.GetApprovedBusinessesAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetRejectedBusinesses")]
        public async Task<IActionResult> GetRejectedBusinesses()
        {
            try
            {
                var result = await _dashboardService.GetRejectedBusinessesAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetTotalJobs")]
        public async Task<IActionResult> GetTotalJobs()
        {
            try
            {
                var result = await _dashboardService.GetTotalJobsAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetTotalCandidates")]
        public async Task<IActionResult> GetTotalCandidates()
        {
            try
            {
                var result = await _dashboardService.GetTotalCandidatesAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetTodayRegistrations")]
        public async Task<IActionResult> GetTodayRegistrations()
        {
            try
            {
                var result = await _dashboardService.GetTodayRegistrationsAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetActiveUsersCount")]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            try
            {
                var result = await _dashboardService.GetActiveUsersCountAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
