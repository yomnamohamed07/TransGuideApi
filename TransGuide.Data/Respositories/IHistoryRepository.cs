
using System.Threading.Tasks;
using TransGuide.Data.Entities.Identity;


namespace TransGuide.Data.Respositories
{
    public interface IHistoryRepository
    {
        Task<History?> GetHistoryAsync(string userid);
        Task<History?> CreateorUpdateHistoryAsync(History? History, TimeSpan? TimeToLive = null);

        Task<bool> DeleteHistoryAsync(string key);

        Task<bool> DeleteTripFromHistoryAsync(string userId, string tripId);
    }
}
