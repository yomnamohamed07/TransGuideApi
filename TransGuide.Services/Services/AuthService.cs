using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly ResetCodeService _resetCodeService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<UserProfile> userManager,
            ResetCodeService resetCodeService,
            IMapper mapper,
            IConfiguration config)
        {
            _userManager = userManager;
            _resetCodeService = resetCodeService;
            _mapper = mapper;
            _config = config;
        }

        
        public async Task<(bool Succeeded, string Message)> RegisterAsync(RegisterRequest model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return (false, "Email is already in use.");

            var user = _mapper.Map<UserProfile>(model);
            user.UserName = model.Email;

            var result = await _userManager.CreateAsync(user, model.Password);
            return result.Succeeded
                ? (true, "User registered successfully.")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

       
        public async Task<LoginResponse> LoginAsync(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new LoginResponse { Succeeded = false };

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Succeeded = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName ?? ""
            };
        }

       
        public async Task<LoginResponse> GoogleLoginAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _config["Google:ClientId"] }
                    });
            }
            catch
            {
                return new LoginResponse { Succeeded = false };
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new UserProfile
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FullName = payload.Name,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return new LoginResponse { Succeeded = false };
            }

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Succeeded = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName ?? ""
            };
        }

        public async Task<(bool Succeeded, string Message)> UpdateUserDataAsync(string userId, UpdateUserDataRequest model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;
            if (!string.IsNullOrEmpty(model.Country))
                user.Country = model.Country;
            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? (true, "User data has been successfully updated.")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

   
        public async Task<(bool Succeeded, string Message)> UpdatePasswordAsync(string userId, UpdatePasswordRequest model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isPasswordValid)
                return (false, "Current password is incorrect.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            return result.Succeeded
                ? (true, "Password updated successfully.")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<(bool Succeeded, string Message)> ForgotPasswordAsync(ForgotPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "User not found.");

            var code = _resetCodeService.GenerateCode(model.Email);
            Console.WriteLine($"[DEV] Reset code for {model.Email}: {code}");

            return (true, "Password reset code sent to your email.");
        }

        public Task<bool> VerifyResetCodeAsync(string email, string code)
        {
            return Task.FromResult(_resetCodeService.IsValid(email, code));
        }

        
        public async Task<(bool Succeeded, string Message)> ResetPasswordAsync(ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "User not found.");

            if (!_resetCodeService.IsValid(model.Email, model.Code))
                return (false, "Invalid or expired reset code.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            _resetCodeService.RemoveCode(model.Email);

            return result.Succeeded
                ? (true, "Password has been reset successfully.")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        
        private string GenerateJwtToken(UserProfile user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim("FullName", user.FullName ?? "")
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:ExpiresMinutes"])),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}