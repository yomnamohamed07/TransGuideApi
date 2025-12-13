using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TransGuide.Data.Entities.Identity;
using TransGuideApi.Models;
using TransGuideApi.Services;

namespace TransGuideApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly ResetCodeService _resetCodeService;
        public AuthController(
            UserManager<UserProfile> userManager,
            ResetCodeService resetCodeService)
        {
            _userManager = userManager;
            _resetCodeService = resetCodeService;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email is already in use.");

            var user = new UserProfile
            {
                Email = model.Email,
                UserName = model.Email, // Identity بيحطّلنا UserName مطلوب
                FullName = model.FullName,
                Country = model.Country,
                Address = model.Address,
                CurrentLatitude = model.CurrentLatitude,
                CurrentLongitude = model.CurrentLongitude
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }
    

    [HttpPost("signin")]
        public async Task<IActionResult> Signin(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("The email or password is incorrect.");
            }

            // إنشاء الـ JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ThisIsASecureKeyForTransiGuide!2025");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FullName", user.FullName ?? "")
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "TransiGuide",
                Audience = "TransiGuideUsers",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                userId = user.Id,
                email = user.Email,
                fullName = user.FullName
            });
        }
        [HttpPost("update-logged-user-data")]
        [Authorize] 
        public async Task<IActionResult> UpdateLoggedUserData(UpdateUserDataRequest model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Cannot identify the user.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // حدّث البيانات لو اتت في الطلب
            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrEmpty(model.Country))
                user.Country = model.Country;

            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            // حدّث الموقع الجغرافي دائمًا (حتى لو نفس القيمة)
            user.CurrentLatitude = model.CurrentLatitude;
            user.CurrentLongitude = model.CurrentLongitude;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User data has been successfully updated." });
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [HttpPost("update-logged-user-password")]
        [Authorize]
        public async Task<IActionResult> UpdateLoggedUserPassword(UpdatePasswordRequest model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Unauthorized.");

            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
                return NotFound("User not found.");

            // Verify current password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isPasswordValid)
                return BadRequest("Current password is incorrect.");

            // Update password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "Password updated successfully." });
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found.");

            // ولّد كود واطبعه في الـ Console (للتطوير)
            var code = _resetCodeService.GenerateCode(model.Email);
            Console.WriteLine($"[DEV] Reset code for {model.Email}: {code}");

            // في الإنتاج: هنرسل إيميل هنا
            return Ok(new { message = "Password reset code sent to your email." });
        }

        [HttpPost("verify-reset-code")]
        public IActionResult VerifyResetCode(VerifyResetCodeRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var isValid = _resetCodeService.IsValid(model.Email, model.Code);
            if (!isValid)
                return BadRequest("Invalid or expired reset code.");

            return Ok(new { message = "Reset code is valid." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found.");

            if (!_resetCodeService.IsValid(model.Email, model.Code))
                return BadRequest("Invalid or expired reset code.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            _resetCodeService.RemoveCode(model.Email); 

            if (result.Succeeded)
                return Ok(new { message = "Password has been reset successfully." });

            return BadRequest(result.Errors.Select(e => e.Description));
        }
    } }