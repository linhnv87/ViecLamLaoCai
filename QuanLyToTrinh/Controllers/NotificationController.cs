using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("GetNotificationsForUser/{userId}")]
        public async Task<IActionResult> GetNotificationsForUser(string userId)
        {
            try
            {
                var result = await _notificationService.GetNotificationsForUser(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex) 
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("CreateNotification/{type}/{docId}/{userId?}")]
        public async Task<IActionResult> CreateNotification(int type, int docId, string? userId)
        {
            try
            {
                var result = await _notificationService.CreateNotifications(type, docId, userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("UpdateNotificationStatus/{id}")]
        public async Task<IActionResult> UpdateNotificationStatus(int id)
        {
            try
            {
                var result = await _notificationService.UpdateNotificationStatus(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("CheckNewAlert/{userId}")]
        public async Task<IActionResult> CheckNewAlert(string userId)
        {
            try
            {
                var result = await _notificationService.CheckNewAlert(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
