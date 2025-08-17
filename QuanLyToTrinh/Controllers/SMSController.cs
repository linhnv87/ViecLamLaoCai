using Core.Domains;
using Core.QueryModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyToTrinh.SMSService;
using Services;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly ISMSService _smsService;
        private readonly ISMSLogService _logService;
        public SMSController(ISMSService smsService, ISMSLogService logService)
        {
            _smsService = smsService;
            _logService = logService;
        }

        [HttpGet("SendSMS/{docId}/{type}")]
        public async Task<IActionResult> SendSMS(int docId, int type)
        {
            try
            {
                var result = await _smsService.SendSMS(docId, type);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("SendSMSV2/{docId}/{type}")]
        public async Task<IActionResult> SendSMSV2(int docId, int type)
        {
            try
            {
                var result = await _smsService.SendSMSV2(docId, type);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpPost("GetAllLogsWithUserNames")]
        public async Task<IActionResult> GetAllLogsWithUserNames([FromBody] SMSLogQueryModel queryModel)
        {
            try
            {
                bool? isSucceeded = queryModel.IsSucceeded;

                var result = await _logService.GetAllLogsWithUserNames(queryModel);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("TestSendSMS")]
        public async Task<IActionResult> TestSendSMS()
        {
            try
            {
                var result = await _smsService.TestSMSSending();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
