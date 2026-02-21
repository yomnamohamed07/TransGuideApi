using TransGuide.Services.Models;
using TransGuide.Services.Modelsls;

namespace TransGuide.Services.Services
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string Message)> RegisterAsync(RegisterRequest model);
        Task<(bool Succeeded, string Token, int UserId, string Email, string FullName)> LoginAsync(LoginRequest model);
        Task<(bool Succeeded, string Message)> UpdateUserDataAsync(string userId, UpdateUserDataRequest model);
        Task<(bool Succeeded, string Message)> UpdatePasswordAsync(string userId, UpdatePasswordRequest model);
        Task<(bool Succeeded, string Message)> ForgotPasswordAsync(ForgotPasswordRequest model);
        Task<bool> VerifyResetCodeAsync(string email, string code);
        Task<(bool Succeeded, string Message)> ResetPasswordAsync(ResetPasswordRequest model);
    }
}