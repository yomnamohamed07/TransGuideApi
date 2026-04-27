
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Data.Services
{
    public interface IGeoLocationService
    {
        Task AddStationsAsync(IEnumerable<Station> stations);
        Task<Station?> GetNearestStationAsync(decimal lat, decimal lon);
    }
}
