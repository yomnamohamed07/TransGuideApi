
using TransGuide.Data.MappingProfiles;

namespace TransGuide.Services
{
    public interface IHistoryServices
    {
        Task<HistoryDto> GetHistoryAsync(string UserId);
        Task<HistoryDto> CreateorUpdateHistoryAsync(HistoryDto history);

        Task<bool> DeleteHistoryAsync(string key);
    }
}
