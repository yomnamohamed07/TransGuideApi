
using TransGuide.Data.Entities.ApplicationEntities;
namespace TransGuide.Data.Repositories;

public interface IRatingRepository : IGenericRepository<Rating>
{
    Task<IEnumerable<Rating>> GetRatingsByTripIdAsync(int tripId);
    Task<double> GetAverageRatingForTripAsync(int tripId);
    Task<double> GetOverallAverageAsync();

}