using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class VoiceServices : IVoiceServices
    {
        private readonly HttpClient _httpClient;

        public VoiceServices()
        {
            _httpClient = new HttpClient();
        }
        public async Task<string> SendVoiceAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();

            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync(
                "https://amr-yasserr-ara-route-parser.hf.space/predict",
                content
            );

            return await response.Content.ReadAsStringAsync();
        }
    }
}

