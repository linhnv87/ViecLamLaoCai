using Core.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using System.Net.Http.Headers;
using System.Net;
using static Services.DTO.SSODTOs;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSOController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAppUserService _userService;
        private readonly ILogger _logger;

        public SSOController(IConfiguration configuration, IAppUserService userService, ILogger<SSOController> logger)
        {
            _configuration = configuration;
            _userService = userService;
            _logger = logger;
        }

        private string authority { get { return _configuration["AppSettings:Authority"]?.ToString() ?? ""; } }
        private string clientId { get { return _configuration["AppSettings:ClientId"]?.ToString() ?? ""; } }
        private string clientSecret { get { return _configuration["AppSettings:ClientSecret"]?.ToString() ?? ""; } }
        private string clientScope { get { return _configuration["AppSettings:ClientScope"]?.ToString() ?? "openid"; } }
        private string redirect_uri { get { return _configuration["AppSettings:RedirectUri"]?.ToString() ?? ""; } }

        [HttpGet("get-auth-uri")]
        public IActionResult GetSsoAuthUri()
        {
            try
            {
                string ssoAuthUrl = "";
                if (authority != "" && clientId != "")
                {
                    ssoAuthUrl = $"{authority}/oauth2/authorize?response_type=code&client_id={clientId}&redirect_uri={redirect_uri}&scope={clientScope}";
                }
                return Ok(new BaseResponseModel(new
                {
                    authority,
                    auth_url = ssoAuthUrl,
                }));                
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    exception = ex.InnerException?.Message ?? "",
                });
            }
        }

        [HttpPost("signin")]        
        public async Task<IActionResult> SsoSinginOidcCallback([FromBody] SsoPostData data)
        {
            string origin = "";
            string response = "";
            try
            {
                if (string.IsNullOrWhiteSpace(data.code))
                    throw new Exception("code is empty");
                if (string.IsNullOrWhiteSpace(data.redirect_uri))
                    throw new Exception("redirect_uri is empty");
                if (authority != "" && clientId != "")
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("code", data.code),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                        new KeyValuePair<string, string>("redirect_uri", data.redirect_uri)
                    });

                    var tokenResponse = await client.PostAsync(authority + "/oauth2/token", formData);

                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        // OK, tiếp tục lấy UserInfo để ghép
                        var tokenResultString = await tokenResponse.Content.ReadAsStringAsync();
                        var tokenResult = JsonConvert.DeserializeObject<TokenResponseData>(tokenResultString);
                        _logger.LogInformation(tokenResult.access_token);
                        //client.DefaultRequestHeaders.Remove("Content-Type");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.access_token);
                        var userInfoResponse = await client.PostAsync(authority + "/oauth2/userinfo", null);
                        if (userInfoResponse.IsSuccessStatusCode)
                        {
                            var userInfoResultString = await userInfoResponse.Content.ReadAsStringAsync();
                            var userInfoResult = JsonConvert.DeserializeObject<UserInfoResponseData>(userInfoResultString);
                            _logger.LogInformation($"userInfoResultString:{userInfoResultString}");
                            var localUser = await _userService.GetUserForSSO(userInfoResult);

                            if (localUser != null)
                            {
                                var user = localUser;                                
                                var accessToken = await _userService.GenerateToken(user);
                                return Ok(new BaseResponseModel(accessToken));
                            }
                            else
                            {
                                return Ok(new BaseResponseModel(new JsonResult(new
                                {
                                    message = "Tài khoản chưa cập nhật thông tin cá nhân trên nền tảng Bàn làm việc, hãy liên hệ quản trị viên để được trợ giúp",
                                    error = "no user",
                                }), false, StatusCodes.Status500InternalServerError, "Tài khoản chưa cập nhật thông tin cá nhân trên nền tảng Bàn làm việc, hãy liên hệ quản trị viên để được trợ giúp"));
                            }
                        }
                        else
                        {
                            // Có vấn đề khi lấy userInfo
                            var errorResponseString = await userInfoResponse.Content.ReadAsStringAsync();
                            var errorResponse = JsonConvert.DeserializeObject<SsoErrorResponse>(errorResponseString);
                            origin = "userinfo";
                            response = errorResponseString;
                            throw new Exception(errorResponse.error, new Exception(errorResponse.error_description));
                        }
                    }
                    else
                    {
                        // Có vấn đề khi lấy access token
                        var errorResponseString = await tokenResponse.Content.ReadAsStringAsync();
                        var errorResponse = JsonConvert.DeserializeObject<SsoErrorResponse>(errorResponseString);
                        origin = "token";
                        response = errorResponseString;
                        throw new Exception(errorResponse.error, new Exception(errorResponse.error_description));
                    }
                }
                else
                {
                    throw new Exception("Client này chưa cấu hình SSO");
                }
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(new JsonResult(new
                {
                    message = ex.Message,
                    exception = ex.InnerException?.Message ?? "",
                    origin,
                    response,
                }), false, StatusCodes.Status500InternalServerError, ex.Message));                
            }
        }
    }        
}
