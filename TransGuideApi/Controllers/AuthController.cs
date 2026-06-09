using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.MappingProfiles.Outputs;
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

        // ================= GET CURRENT USER =================
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized" });

            var user = await _authService.GetCurrentUserAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // ================= UPDATE CURRENT USER =================
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UserProfileDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized" });

            var (succeeded, message) = await _authService.UpdateCurrentUserAsync(userId, model);

            return succeeded
                ? Ok(new { message })
                : BadRequest(new { message });
        }

        [HttpPost("send-reset-code")]
        public async Task<IActionResult> SendResetCode([FromBody] SendResetPasswordRequest request)
        {
            var result = await _authService.SendResetPasswordCode(request.Email);
            return Ok(result);
        }

        [HttpPost("confirm-reset-code")]
        public async Task<IActionResult> ConfirmResetCode([FromBody] ConfirmResetCodeRequest request)
        {
            var result = await _authService.ConfirmResetCode(request.Email, request.Code);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] TransGuide.Data.MappingProfiles.Inputs.ResetPasswordRequest request)
        {
            var result = await _authService.ResetPassword(request.Email, request.Password);
            return Ok(result);
        }

        [HttpGet("UsersCount")]
        public async Task<IActionResult> GetUsersCount()
        {
            var count = await _authService.GetUsersCount();
            return Ok(new { Count = count });
       }

    }
}