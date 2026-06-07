using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using TransGuide.Data.MappingProfiles.Inputs;

public class AiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public AiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;

        _http.Timeout = TimeSpan.FromSeconds(5);
    }

    public async Task<PredictionResponse> PredictAsync(
        Guid sessionId,
        float[] landmarks)
    {
        try
        {
            var url = _config["AI:Url"];

            var body = new
            {
                session_id = sessionId,
                landmarks = landmarks
            };

            var json = JsonSerializer.Serialize(body);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return new PredictionResponse
                {
                    status = "ERROR_AI"
                };
            }

            var result = await response.Content.ReadAsStringAsync();

            var prediction = JsonSerializer.Deserialize<PredictionResponse>(
                result,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return prediction ?? new PredictionResponse
            {
                status = "EMPTY_RESPONSE"
            };
        }
        catch
        {
            return new PredictionResponse
            {
                status = "ERROR_AI"
            };
        }
    }
}