
using TransGuide.Data.MappingProfiles;

namespace TransGuide.Services
{
    public interface IHistoryServices
    {
        Task<HistoryDto> GetHistoryAsync(string UserId);
        Task<HistoryDto> CreateorUpdateHistoryAsync(HistoryDto history);
        Task<bool> DeleteHistoryAsync(string key);

        Task<bool> DeleteTripFromHistoryAsync(string userId, string tripId);

        public Task<long> GetTotalTripsAsync();
    }
}
