using TransGuide.Data.MappingProfiles;

namespace TransGuide.Data.Services
{
    public interface IAuthService
    {
       
        Task<(bool Succeeded, string Message)> RegisterAsync(RegisterRequest model);

        Task<LoginResponse> LoginAsync(LoginRequest model);

        Task<LoginResponse> GoogleLoginAsync(string idToken);

        Task<(bool Succeeded, string Message)> UpdateUserDataAsync(string userId, UpdateUserDataRequest model);

        Task<(bool Succeeded, string Message)> UpdatePasswordAsync(string userId, UpdatePasswordRequest model);

        Task<(bool Succeeded, string Message)> ForgotPasswordAsync(ForgotPasswordRequest model);

        Task<bool> VerifyResetCodeAsync(string email, string code);

        Task<(bool Succeeded, string Message)> ResetPasswordAsync(ResetPasswordRequest model);

        public  Task<int> GetUsersCount();
    }
}