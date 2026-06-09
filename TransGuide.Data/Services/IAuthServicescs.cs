using TransGuide.Data.MappingProfiles;
using TransGuide.Data.MappingProfiles.Outputs;

namespace TransGuide.Data.Services
{
    public interface IAuthService
    {
       
        Task<(bool Succeeded, string Message)> RegisterAsync(RegisterRequest model);

        Task<LoginResponse> LoginAsync(LoginRequest model);

        Task<LoginResponse> GoogleLoginAsync(string idToken);

        Task<UserProfileDto?> GetCurrentUserAsync(string userId);

        Task<(bool Succeeded, string Message)> UpdateCurrentUserAsync(string userId, UserProfileDto model);

        Task<string> SendResetPasswordCode(string email);
        Task<string> ConfirmResetCode(string email, string code);
        Task<string> ResetPassword(string email, string password);
        public  Task<int> GetUsersCount();
    }
}