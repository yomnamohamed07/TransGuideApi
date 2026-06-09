using StackExchange.Redis;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Services;

public class GeoLocationService : IGeoLocationService
{
    private readonly IDatabase _redisDb;
    private const string GeoKey = "stations";

    public GeoLocationService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task AddStationsAsync(IEnumerable<Station> stations)
    {
        foreach (var station in stations)
        {
            
            await _redisDb.GeoAddAsync(
                GeoKey,
                (double)station.Longitude,
                (double)station.Latitude,
                station.Id.ToString()
            );

            
            await _redisDb.HashSetAsync(
                $"station:{station.Id}",
                new HashEntry[]
                {
                    new HashEntry("Id", station.Id),
                    new HashEntry("Name", station.Name),
                    new HashEntry("Lat", (double)station.Latitude),
                    new HashEntry("Lon", (double)station.Longitude)
                }
            );
        }
    }

    public async Task<Station?> GetNearestStationAsync(decimal lat, decimal lon)
    {
        var result = await _redisDb.GeoRadiusAsync(
            GeoKey,
            (double)lon,
            (double)lat,
            5,
            GeoUnit.Kilometers,
            count: 1,
            order: Order.Ascending
        );

        var geoEntry = result.FirstOrDefault();

        if (geoEntry.Member.IsNullOrEmpty)
            return null;

        var stationId = geoEntry.Member.ToString();

        var hash = await _redisDb.HashGetAllAsync($"station:{stationId}");

        if (hash.Length == 0)
            return null;

        return new Station
        {
            Id = int.Parse(hash.First(x => x.Name == "Id").Value),
            Name = hash.First(x => x.Name == "Name").Value,
            Latitude = decimal.Parse(hash.First(x => x.Name == "Lat").Value),
            Longitude = decimal.Parse(hash.First(x => x.Name == "Lon").Value)
        };
    }
}