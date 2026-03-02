using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;


namespace TransGuideApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _authService.RegisterAsync(model);
            return succeeded ? Ok(new { message }) : BadRequest(message);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, token, userId, email, fullName) = await _authService.LoginAsync(model);
            if (!succeeded)
                return Unauthorized(token);

            return Ok(new { token, userId, email, fullName });
        }
        [HttpPost("update-logged-user-data")]
        [Authorize]
        public async Task<IActionResult> UpdateLoggedUserData(UpdateUserDataRequest model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Cannot identify the user.");

            var (succeeded, message) = await _authService.UpdateUserDataAsync(userId, model);
            return succeeded ? Ok(new { message }) : BadRequest(message);
        }

        [HttpPost("update-logged-user-password")]
        [Authorize]
        public async Task<IActionResult> UpdateLoggedUserPassword(UpdatePasswordRequest model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Unauthorized.");

            var (succeeded, message) = await _authService.UpdatePasswordAsync(userId, model);
            return succeeded ? Ok(new { message }) : BadRequest(message);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var (succeeded, message) = await _authService.ForgotPasswordAsync(model);
            return succeeded ? Ok(new { message }) : BadRequest(message);
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var isValid = await _authService.VerifyResetCodeAsync(model.Email, model.Code);
            if (!isValid)
                return BadRequest("Invalid or expired reset code.");

            return Ok(new { message = "Reset code is valid." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var (succeeded, message) = await _authService.ResetPasswordAsync(model);
            return succeeded ? Ok(new { message }) : BadRequest(message);
        }
    } 
}