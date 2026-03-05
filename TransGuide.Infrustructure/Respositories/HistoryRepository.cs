
using StackExchange.Redis;
using System.Text.Json;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Respositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly IDatabase _database;

    public HistoryRepository(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    private string BuildKey(string userId)
        => $"history:{userId}";

    public async Task<History?> GetHistoryAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        var key = BuildKey(userId);
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return null;

        try
        {
            return JsonSerializer.Deserialize<History>(value!);
        }
        catch
        {
            return null; 
        }
    }

    public async Task<History?> CreateorUpdateHistoryAsync(
        History? history,
        TimeSpan? ttl = null)
    {
        if (history is null || string.IsNullOrWhiteSpace(history.UserId))
            return null;

        history.Trips ??= new List<Trip>();

        var key = BuildKey(history.UserId);
        var json = JsonSerializer.Serialize(history);

        var saved = await _database.StringSetAsync(
            key,
            json,
            ttl ?? TimeSpan.FromDays(30));

        return saved ? history : null;
    }

    public async Task<bool> DeleteHistoryAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        return await _database.KeyDeleteAsync(BuildKey(userId));
    }
}