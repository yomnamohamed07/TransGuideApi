
using TransGuide.Data.Respositories;
using TransGuide.Data.Entities.Identity;
using StackExchange.Redis;
using System.Text.Json;


namespace TransGuide.Infrustructure.Respositories
{
    public class HistoryRepository(IConnectionMultiplexer connection) : IHistoryRepository
    {
        private readonly IDatabase _database = connection.GetDatabase();

        public async Task<History?> CreateorUpdateHistoryAsync(History? history, TimeSpan? TimeToLive = null)
        {
            var jsonhistory = JsonSerializer.Serialize(history);
            var result = await _database.StringSetAsync(history.UserId, jsonhistory, TimeToLive ?? TimeSpan.FromDays(30));
            if (result)
            {
                return await GetHistoryAsync(history.UserId);
            }
            else
            {
                return null;

            }
        }

            public async Task<bool> DeleteHistoryAsync(string key)
              => await _database.KeyDeleteAsync(key);


            public async Task<History?> GetHistoryAsync(string userid)
            {
                var key = userid.ToString();
                var history = await _database.StringGetAsync(key);
                if (history.IsNullOrEmpty)
                {
                    return null;
                }
                else
                {
                    return JsonSerializer.Deserialize<History>(history!);
                }
            }
        }
    }
