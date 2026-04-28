using Microsoft.AspNetCore.Http;


namespace TransGuide.Data.Services
{
    public interface IVoiceServices
    {
        public Task<string> SendVoiceAsync(IFormFile file);
    }
}
