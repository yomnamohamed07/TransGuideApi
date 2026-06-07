using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<UserProfile> userManager,
            IEmailService emailService,
            IMapper mapper,
            IConfiguration config)
        {
            _userManager = userManager;
            _emailService = emailService;
            _mapper = mapper;
            _config = config;
        }

        // ================= REGISTER =================
        public async Task<(bool Succeeded, string Message)> RegisterAsync(RegisterRequest model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return (false, "Email already exists");

            var user = _mapper.Map<UserProfile>(model);
            user.UserName = model.Email;

            var result = await _userManager.CreateAsync(user, model.Password);

            return result.Succeeded
                ? (true, "Registered successfully")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ================= LOGIN =================
        public async Task<LoginResponse> LoginAsync(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new LoginResponse { Succeeded = false };

            var token = await GenerateJwtToken(user);

            return new LoginResponse
            {
                Succeeded = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName ?? ""
            };
        }
        // ================= GOOGLE LOGIN =================
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

            var token = await GenerateJwtToken(user);

            return new LoginResponse
            {
                Succeeded = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName ?? ""
            };
        }

        // ================= GET =================
        public async Task<UserProfileDto?> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            return _mapper.Map<UserProfileDto>(user);
        }

        // ================= UPDATE =================
        public async Task<(bool Succeeded, string Message)> UpdateCurrentUserAsync(string userId, UserProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return (false, "User not found");

            _mapper.Map(model, user);

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? (true, "Updated successfully")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }



        // ================= JWT GENERATION =================
        private async Task<string> GenerateJwtToken(UserProfile user)
        {
            var key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FullName", user.FullName ?? "")
            };

        
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:ExpiresMinutes"])),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ================= USERS COUNT =================
        public async Task<int> GetUsersCount()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<string> SendResetPasswordCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return "UserNotFound";

            var code = GenerateCode();

            user.ResetCode = code;
            user.ResetCodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await _userManager.UpdateAsync(user);

            await _emailService.SendEmail(
                user.Email,
                BuildEmail(code),
                "Reset Password Code"
            );

            return "Success";
        }

        public async Task<string> ConfirmResetCode(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return "UserNotFound";

            if (user.ResetCode == null || user.ResetCodeExpiry == null)
                return "NoCodeGenerated";

            if (user.ResetCodeExpiry < DateTime.UtcNow)
                return "ExpiredCode";

            if (user.ResetCode != code)
                return "InvalidCode";

            return "Success";
        }

        public async Task<string> ResetPassword(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return "UserNotFound";

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
                return "Failed";

            user.ResetCode = null;
            user.ResetCodeExpiry = null;

            await _userManager.UpdateAsync(user);

            return "Success";
        }

        // helpers
        private string GenerateCode()
        {
            var random = new Random();

            return new string(Enumerable.Range(0, 6)
                .Select(_ => "0123456789"[random.Next(10)]).ToArray());
        }

        private string BuildEmail(string code)
        {
            return $@"
            <h2>Password Reset</h2>
            <p>Your OTP code is:</p>
            <h1 style='color:red'>{code}</h1>
            <p>Valid for 10 minutes</p>
        ";
        }
    }
}