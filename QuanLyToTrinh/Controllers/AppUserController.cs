using Aspose.Pdf.Operators;
using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService _userService;
        private readonly ILogger<AppUserController> _logger;
        public AppUserController(IAppUserService userService, ILogger<AppUserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LogInDTO payload)
        {
            try
            {
                var result = await _userService.UserLogin(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpDTO payload)
        {
            try
            {
                var result = await _userService.UserSignUp(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch(Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassWord(ChangePasswordDTO payload)
        {
            try
            {
                var result = await _userService.ChangePassword(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {             
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("ResetPassword/{userId}")]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            try
            {
                var result = await _userService.ResetPassword(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetAllUserInfo")]
        public async Task<IActionResult> GetAllUserInfo()
        {
            try
            {
                var result = await _userService.GetAllInfoUser();
                return Ok(new BaseResponseModel(result));
            }
            catch(Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetAllSpecialist")]
        public async Task<IActionResult> GetAllSpecialistInfo()
        {
            try
            {
                var result = await _userService.GetAllSpecialistInfoUser();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var result = await _userService.GetUserById(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("GetUserWithRoles")]
        public async Task<IActionResult> GetUserWithRoles()
        {
            try
            {
                var result = await _userService.GetUserWithRolesAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }

        }
        [HttpGet("GetUserByRole/{roleName}")]
        public async Task<IActionResult> GetUserByRole(string roleName)
        {
            try
            {
                var result = await _userService.GetUsersByRoleAsync(roleName);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserInfoDTO payload)
        {
            try
            {
                var result = await _userService.UpdateInfo(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("Lock/{id}")]
        public async Task<IActionResult> LockUser(string id)
        {
            try
            {
                var result = await _userService.LockUser(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {                
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
