using Microsoft.AspNetCore.Authorization;
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
            return succeeded ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(model);
            if (!response.Succeeded)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(new
            {
                token = response.Token,
                userId = response.UserId,
                email = response.Email,
                fullName = response.FullName
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.GoogleLoginAsync(model.IdToken);
            if (!response.Succeeded)
                return Unauthorized(new { message = "Invalid Google token." });

            return Ok(new
            {
                token = response.Token,
                userId = response.UserId,
                email = response.Email,
                fullName = response.FullName
            });
        }

        [HttpPost("update-logged-user-data")]
        [Authorize]
        public async Task<IActionResult> UpdateLoggedUserData(UpdateUserDataRequest model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Cannot identify the user." });

            var (succeeded, message) = await _authService.UpdateUserDataAsync(userId, model);
            return succeeded ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("update-logged-user-password")]
        [Authorize]
        public async Task<IActionResult> UpdateLoggedUserPassword(UpdatePasswordRequest model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized." });

            var (succeeded, message) = await _authService.UpdatePasswordAsync(userId, model);
            return succeeded ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input data." });

            var (succeeded, message) = await _authService.ForgotPasswordAsync(model);
            return succeeded ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input data." });

            var isValid = await _authService.VerifyResetCodeAsync(model.Email, model.Code);
            if (!isValid)
                return BadRequest(new { message = "Invalid or expired reset code." });

            return Ok(new { message = "Reset code is valid." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input data." });

            var (succeeded, message) = await _authService.ResetPasswordAsync(model);
            return succeeded ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpGet("UsersCount")]
        public async Task<IActionResult> GetUsersCount()
        {
            var count = await _authService.GetUsersCount();
            return Ok(new { Count = count });
       }

    }
}