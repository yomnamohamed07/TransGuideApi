using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using TransGuide.Data.MappingProfiles.Outputs;
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

            var res = await _http.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            return await res.Content.ReadAsStringAsync();
        }
       
    }
}