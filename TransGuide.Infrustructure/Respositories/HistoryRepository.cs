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

    public async Task<long> GetTotalTripsCountAsync()
    {
        var server = _database.Multiplexer.GetServer(
            _database.Multiplexer.GetEndPoints().First()
        );

        var keys = server.Keys(pattern: "history:*");

        long totalTrips = 0;

        foreach (var key in keys)
        {
            var data = await _database.StringGetAsync(key);

            if (data.IsNullOrEmpty)
                continue;

            var history = JsonSerializer.Deserialize<History>(data!);

            if (history?.Trips != null)
                totalTrips += history.Trips.Count;
        }

        return totalTrips;
    }

 

    public async Task<History?> GetHistoryAsync(string userId)
    {
        var value = await _database.StringGetAsync(BuildKey(userId));

        if (value.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<History>(value!);
    }

    public async Task<History?> CreateorUpdateHistoryAsync(History history, TimeSpan? ttl = null)
    {
        if (history is null || string.IsNullOrWhiteSpace(history.UserId))
            return null;

        history.Trips ??= new List<Trip>();

        var json = JsonSerializer.Serialize(history);

        var saved = await _database.StringSetAsync(
            BuildKey(history.UserId),
            json,
            ttl ?? TimeSpan.FromDays(30));

        return saved ? history : null;
    }

    public async Task<bool> DeleteHistoryAsync(string userId)
    {
        return await _database.KeyDeleteAsync(BuildKey(userId));
    }

    public async Task<bool> DeleteTripFromHistoryAsync(string userId, string tripId)
    {
        var data = await _database.StringGetAsync(BuildKey(userId));

        if (data.IsNullOrEmpty)
            return false;

        var history = JsonSerializer.Deserialize<History>(data!);

        if (history?.Trips == null)
            return false;

        var trip = history.Trips.FirstOrDefault(t => t.Id == tripId);

        if (trip == null)
            return false;

        history.Trips.Remove(trip);

        await _database.StringSetAsync(
            BuildKey(userId),
            JsonSerializer.Serialize(history)
        );

        return true;
    }
}