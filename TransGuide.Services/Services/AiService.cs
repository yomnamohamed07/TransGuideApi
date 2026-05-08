using Microsoft.Extensions.Configuration;
using System.Text;
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
        }

        public async Task<string> Predict(string frame)
        {
            var url = _config["AI:Url"];

            var json = JsonSerializer.Serialize(new
            {
                data = new[] { frame }
            });

            Console.WriteLine("📡 Calling AI...");

            var res = await _http.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (!res.IsSuccessStatusCode)
            {
                var error = await res.Content.ReadAsStringAsync();
                throw new Exception($"AI Error: {res.StatusCode} - {error}");
            }

            var result = await res.Content.ReadAsStringAsync();

            Console.WriteLine($"🤖 AI Result: {result}");

            return result;
        }
    }
}