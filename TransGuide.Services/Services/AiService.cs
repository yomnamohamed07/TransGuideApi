using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace TransGuide.Services.Services
{
    public class AiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _http.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<string> Predict(byte[] imageBytes)
        {
            var url = _config["AI:Url"];

            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(imageBytes), "file", "frame.jpg");

            var response = await _http.PostAsync(url, content);

            var json = await response.Content.ReadAsStringAsync();

            var result =
                JsonSerializer.Deserialize<AiPredictionResponse>(json);

            return result?.Prediction ?? "";
        }
    }

    public class AiPredictionResponse
    {
        public string Prediction { get; set; } = "";
    }
}