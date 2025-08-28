using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;
using System.Threading.Tasks;

namespace QuanLyToTrinh.Controllers.Website
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        // GET /api/Home/GetHomePageData
        [HttpGet("GetHomePageData")]
        public async Task<IActionResult> GetHomePageData()
        {
            try
            {
                var result = await _homeService.GetHomePageDataAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetFeaturedJobs/{count?}
        [HttpGet("GetFeaturedJobs/{count?}")]
        public async Task<IActionResult> GetFeaturedJobs(int? count = null)
        {
            try
            {
                var result = await _homeService.GetFeaturedJobsAsync(count);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetSuggestedJobs?count={count}&userId={userId}
        [HttpGet("GetSuggestedJobs")]
        public async Task<IActionResult> GetSuggestedJobs(int? count = null, string userId = null)
        {
            try
            {
                var result = await _homeService.GetSuggestedJobsAsync(userId, count);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetJobCategories
        [HttpGet("GetJobCategories")]
        public async Task<IActionResult> GetJobCategories()
        {
            try
            {
                var result = await _homeService.GetJobCategoriesAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetHomeStats
        [HttpGet("GetHomeStats")]
        public async Task<IActionResult> GetHomeStats()
        {
            try
            {
                var result = await _homeService.GetHomeStatsAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetLatestJobs/{count?}
        [HttpGet("GetLatestJobs/{count?}")]
        public async Task<IActionResult> GetLatestJobs(int? count = null)
        {
            try
            {
                var result = await _homeService.GetLatestJobsAsync(count);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetFeaturedCompanies/{count?}
        [HttpGet("GetFeaturedCompanies/{count?}")]
        public async Task<IActionResult> GetFeaturedCompanies(int? count = null)
        {
            try
            {
                var result = await _homeService.GetFeaturedCompaniesAsync(count);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // POST /api/Home/SearchJobs
        [HttpPost("SearchJobs")]
        public async Task<IActionResult> SearchJobs([FromBody] SearchJobsRequestDTO searchQuery)
        {
            try
            {
                var result = await _homeService.SearchJobsAsync(searchQuery);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        // GET /api/Home/GetPopularSearches/{count?}?period={period}
        [HttpGet("GetPopularSearches/{count?}")]
        public async Task<IActionResult> GetPopularSearches(int? count = null, string period = null)
        {
            try
            {
                var result = await _homeService.GetPopularSearchesAsync(count, period);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
